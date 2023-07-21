using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Services;
using Microsoft.Graph;
using Beta = Microsoft.Graph.Beta;

namespace GraphApiLeastPrivilegeManagement.Tests.Fixtures;
public class AdminFixture: IDisposable
{
    public readonly string TenantId;
    public readonly string ClientId;
    public readonly GraphApiAuthenticationService AuthenticationService;
    public readonly GraphServiceClient GraphCli;
    public readonly Beta.GraphServiceClient BetaGraphCli;
    public readonly AzurePrincipal AdminPrincipal;
    
    public AdminFixture()
    {
        TenantId = Environment.GetEnvironmentVariable("ARM_TENANT_ID") ?? "";
        ClientId = Environment.GetEnvironmentVariable("ARM_CLIENT_ID") ?? "";
        var clientSecret = Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET") ?? "";
        
        AuthenticationService = new GraphApiAuthenticationService(
            TenantId,
            ClientId,
            clientSecret
        );
        GraphCli = new GraphServiceClient(
            AuthenticationService.GetCredentials(),
            AuthenticationService.GetScopes()
        );
        BetaGraphCli = new Beta.GraphServiceClient(
            AuthenticationService.GetCredentials(),
            AuthenticationService.GetScopes()
        );
        
        // Get the admin service principal
        //https://learn.microsoft.com/en-us/graph/api/application-list?view=graph-rest-1.0&tabs=csharp#request-2
        var result = GraphCli.ServicePrincipals.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Filter = $"appId eq '{ClientId}'";
            requestConfiguration.QueryParameters.Select = new string []{ "id", "appId", "displayName" };
        }).Result;

        if (result?.Value?.Count != 1) throw new Exception("Unable to find admin service principal");
        var caller = result.Value.FirstOrDefault();
        AdminPrincipal = new AzurePrincipal
        {
            Id = caller!.Id!,
            DisplayName = caller.DisplayName!,
            PrincipalType = PrincipalType.ServicePrincipal
        };
    }

    public void Dispose()
    {
        GraphCli.Dispose();
        GraphCli.Dispose();
    }
}