using System.Text.Json;
using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Utilities;
using Microsoft.Graph;

using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Beta = Microsoft.Graph.Beta;
using DirectoryObject = Microsoft.Graph.Beta.Models.DirectoryObject;
using User = Microsoft.Graph.Beta.Models.User;
using ServicePrincipal = Microsoft.Graph.Beta.Models.ServicePrincipal;

namespace GraphApiLeastPrivilegeManager.Services.Groups;

public class GraphApiGroupService : IGraphApiGroupService
{
    private readonly ILogger<GraphApiGroupService> _logger;
    private readonly GraphServiceClient _graphCli;
    private readonly Beta.GraphServiceClient _betaGraphCli;

    public GraphApiGroupService(ILogger<GraphApiGroupService> logger, IGraphApiAuthenticationService auth)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var authService = auth ?? throw new ArgumentNullException(nameof(auth));
        var scopes = new[] { "Group.ReadWrite.All" };
        //Using beta due to know issue with service principals not being listed as group owners
        var baseUrl = "https://graph.microsoft.com/v1.0";
        var betaUrl = "https://graph.microsoft.com/beta";
        _graphCli = new GraphServiceClient(authService.GetCredentials(), authService.GetScopes() ?? scopes, baseUrl);
        _betaGraphCli = new Beta.GraphServiceClient(
            authService.GetCredentials(),
            authService.GetScopes(),
            betaUrl
        );
    }

    public GraphApiGroupService(
        ILogger<GraphApiGroupService> logger,
        GraphServiceClient graphCli,
        Beta.GraphServiceClient betaGraphCli)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _graphCli = graphCli ?? throw new ArgumentNullException(nameof(graphCli));
        _betaGraphCli = betaGraphCli ?? throw new ArgumentNullException(nameof(betaGraphCli));
        _graphCli = graphCli;
        _betaGraphCli = betaGraphCli;
    }

    #region Group
    //https://learn.microsoft.com/en-us/graph/api/group-list?view=graph-rest-1.0&tabs=csharp#example-4-use-filter-and-top-to-get-one-group-with-a-display-name-that-starts-with-a-including-a-count-of-returned-objects
    public async Task<ServiceResponse<GroupEntity?>> GetGroupByDisplayName(string displayName)
    {
        var result = await _graphCli.Groups.GetAsync(cfg =>
        {
            // cfg.QueryParameters.Count = true; //This request requires the ConsistencyLevel header set to eventual because $count is in the request.
            cfg.QueryParameters.Filter = $"displayName eq '{displayName}'";
            cfg.QueryParameters.Select = new[] { "id", "displayName", "description" };
            // requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
        });

        var group = result?.Value?.FirstOrDefault();
        return group is null
            ? ServiceResponseGenerator.BadRequest<GroupEntity>()
            : ServiceResponseGenerator.Success(new GroupEntity
            {
                Id = group.Id!,
                DisplayName = group.DisplayName!,
                Description = JsonUtils.TryLoadJson(group.Description!, new Dictionary<string, string>())
            });
    }

    public async Task<ServiceResponse<GroupEntity?>> GetGroupById(string id)
    {
        try
        {
            var group = await _graphCli.Groups[id].GetAsync(cfg =>
            {
                cfg.QueryParameters.Select = new[] { "id", "displayName", "description" };
            });

            return ServiceResponseGenerator.Success(new GroupEntity
            {
                Id = group!.Id!,
                DisplayName = group.DisplayName!,
                Description = JsonUtils.TryLoadJson(group.Description!, new Dictionary<string, string>())
            });
        }
        catch (ODataError)
        {
            return ServiceResponseGenerator.BadRequest<GroupEntity>();
        }
    }


    //https://learn.microsoft.com/en-us/graph/api/group-post-groups?view=graph-rest-beta&tabs=csharp#request
    public async Task<ServiceResponse<GroupEntity?>> CreateGroup(AzurePrincipal owner, string displayName, Dictionary<string, string> description)
    {
        var stringDescription = JsonSerializer.Serialize(description);
        var newGroup = new Group
        {
            DisplayName = displayName,
            Description = stringDescription,
            GroupTypes = new List<string> { },
            MailEnabled = false,
            MailNickname = displayName.Replace(" ", "-").ToLower(),
            SecurityEnabled = true,
            AdditionalData = new Dictionary<string, object>
            {
                //directoryObjects not taken from docs, but error debugging: https://learn.microsoft.com/en-us/answers/questions/38563/groupmember-readwrite-all-does-not-work-for-adding
                //Requires User.Read.All to bind user principals
                //Requires Application.Read.All to bind service principals
                { "owners@odata.bind", new List<string>(){$"https://graph.microsoft.com/v1.0/directoryObjects/{owner.Id}"}}
                // { "owners@odata.bind", new List<string>(){$"https://graph.microsoft.com/v1.0/directoryObjects/e51916ae-0b2b-4fb0-9650-99a9adde633f"}} //tobias@tsant.no
                // { "owners@odata.bind", new List<string>(){$"https://graph.microsoft.com/v1.0/directoryObjects/1ae780e5-3ae7-4277-a834-f5545085e411"}} //terraform-service-user
            }

        };
        try
        {
            var group = await _graphCli.Groups.PostAsync(newGroup);
            return ServiceResponseGenerator.Success(new GroupEntity
            {
                Id = group!.Id!,
                DisplayName = group.DisplayName!,
                Description = description
            });
        }
        catch (ODataError)
        {
            return ServiceResponseGenerator.InternalServerError<GroupEntity>("ODataError");
        }
    }

    public async Task<ServiceResponse<GroupEntity?>> UpdateGroup(AzurePrincipal owner, string groupId, string displayName, Dictionary<string, string> description)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<GroupEntity>();
        }
        var stringDescription = JsonSerializer.Serialize(description);
        var requestBody = new Group
        {
            DisplayName = displayName,
            Description = stringDescription
        };
        await _graphCli.Groups[groupId].PatchAsync(requestBody);
        try
        {
            return ServiceResponseGenerator.Success(new GroupEntity
            {
                Id = groupId!,
                DisplayName = displayName,
                Description = description
            });
        }
        catch (ODataError)
        {
            return ServiceResponseGenerator.BadRequest<GroupEntity>();
        }


    }

    //https://learn.microsoft.com/en-us/graph/api/group-delete?view=graph-rest-1.0&tabs=csharp#request
    public async Task<ServiceResponse<object?>> DeleteGroup(AzurePrincipal owner, string groupId)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<object>();
        }
        await _graphCli.Groups[groupId].DeleteAsync();
        //TODO: try catch ODataError -> group does not exist?
        return ServiceResponseGenerator.Success<object>();
    }
    #endregion


    #region GroupOwners
    public async Task<ServiceResponse<object?>> AddGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<object>();
        }
        var requestBody = new ReferenceCreate
        {
            // OdataId = $"https://graph.microsoft.com/v1.0/{principal.PrincipalType.ToPluralString()}/{principal.Id}",
            OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{principalObjectId}"
        };
        //TODO: try catch ODataError -> owner does not exist?
        try
        {
            await _graphCli.Groups[groupId].Owners.Ref.PostAsync(requestBody);
        }
        catch (ODataError)
        {
            return ServiceResponseGenerator.BadRequest<object>("Principal does not exist");
        }


        return ServiceResponseGenerator.Success<object>();
    }

    //https://learn.microsoft.com/en-us/graph/api/group-delete-owners?view=graph-rest-1.0&tabs=csharp#request
    public async Task<ServiceResponse<object?>> RemoveGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<object>();
        }
        try
        {
            await _graphCli.Groups[groupId].Owners[principalObjectId].Ref.DeleteAsync();
            return ServiceResponseGenerator.Success<object>();
        }
        catch (ODataError)
        {
            return ServiceResponseGenerator.BadRequest<object>("Principal does not exist or is not owner");
        }
    }

    public async Task<ServiceResponse<bool>> IsPrincipalGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<bool>();
        }
        return ServiceResponseGenerator.Success(await IsPrincipalGroupOwner(groupId, principalObjectId));
    }

    /// <summary>
    /// If you attempt the query owner for a group that does not exist, it will return false and methods will return `return ServiceResponseGenerator.Forbidden<bool>();`
    /// </summary>
    private async Task<bool> IsPrincipalGroupOwner(string groupId, string principalObjectId)
    {
        try
        {
            var resp = await _graphCli.Groups[groupId].Owners.GetAsync(
                cfg =>
                {
                    cfg.QueryParameters.Filter = $"id eq '{principalObjectId}'";
                    cfg.QueryParameters.Select = new[] { "id", "odataType" };
                });
            return resp is not null && resp.Value!.Count == 1;
        }
        catch (ODataError)
        {
            return false;
        }
    }

    public async Task<ServiceResponse<List<AzurePrincipal>>> ListGroupOwners(AzurePrincipal owner, string groupId)
    {
        if (!await IsPrincipalGroupOwner(groupId, owner.Id))
        {
            return ServiceResponseGenerator.Forbidden<List<AzurePrincipal>>()!;
        }
        var ret = new List<AzurePrincipal>();
        var result = await _betaGraphCli.Groups[groupId].Owners.GetAsync(
            cfg =>
            {
                //TODO: (performance) With this AppDisplayName, AppId & ServicePrincipalType is null. Beta issue?
                // cfg.QueryParameters.Select = new[] { "DisplayName", "Id", "OdataType", "AppDisplayName", "AppId", "ServicePrincipalType" };
            });
        foreach (var principal in result?.Value ?? new List<DirectoryObject>())
        {
            switch (principal)
            {
                case User u:
                    ret.Add(new AzurePrincipal
                    {
                        Id = u.Id!,
                        DisplayName = u.DisplayName!,
                        PrincipalType = PrincipalType.User
                    });
                    break;
                case ServicePrincipal sp:
                    ret.Add(new AzurePrincipal
                    {
                        Id = sp.Id!,
                        DisplayName = sp.AppDisplayName!,
                        PrincipalType = PrincipalType.ServicePrincipal //TODO -> sp.servicePrincipalType logic
                    });
                    break;
                default:
                    return ServiceResponseGenerator.InternalServerError<List<AzurePrincipal>>("AzurePrincipal is neither of type ServicePrincipal nor User")!;
            }
        }

        return ServiceResponseGenerator.Success(ret)!;
    }

    //https://learn.microsoft.com/en-us/graph/api/group-post-owners?view=graph-rest-1.0&tabs=csharp#request


    #endregion



    #region GroupMembership
    public Task<ServiceResponse<object?>> AddGroupMember(AzurePrincipal owner, string groupId, string principalObjectId) => throw new NotImplementedException();

    public Task<ServiceResponse<object?>> RemoveGroupMember(AzurePrincipal owner, string groupId, string principalObjectId) => throw new NotImplementedException();

    public Task<ServiceResponse<bool>> IsGroupMember(string groupId, string principalObjectId) => throw new NotImplementedException();

    public Task<ServiceResponse<List<AzurePrincipal>>> ListGroupMembers(string groupId) => throw new NotImplementedException();

    #endregion
}