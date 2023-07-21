using GraphApiLeastPrivilegeManager.Models.Groups;

namespace GraphApiLeastPrivilegeManager.Services.Groups;

public interface IGraphApiGroupService
{
    public Task<ServiceResponse<GroupEntity?>> GetGroupByDisplayName(string displayName);
    public Task<ServiceResponse<GroupEntity?>> GetGroupById(string groupId);
    public Task<ServiceResponse<GroupEntity?>> CreateGroup(AzurePrincipal owner, string displayName, Dictionary<string, string> description);
    public Task<ServiceResponse<GroupEntity?>> UpdateGroup(AzurePrincipal owner, string groupId, string displayName, Dictionary<string, string> description);
    public Task<ServiceResponse<object?>> DeleteGroup(AzurePrincipal owner, string groupId);
    public Task<ServiceResponse<object?>> AddGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId);
    public Task<ServiceResponse<object?>> RemoveGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId);
    public Task<ServiceResponse<bool>> IsPrincipalGroupOwner(AzurePrincipal owner, string groupId, string principalObjectId);
    public Task<ServiceResponse<List<AzurePrincipal>>> ListGroupOwners(AzurePrincipal owner, string groupId);
    public Task<ServiceResponse<object?>> AddGroupMember(AzurePrincipal owner, string groupId, string principalObjectId);
    public Task<ServiceResponse<object?>> RemoveGroupMember(AzurePrincipal owner, string groupId, string principalObjectId);
    public Task<ServiceResponse<bool>> IsGroupMember(string groupId, string principalObjectId);
    public Task<ServiceResponse<List<AzurePrincipal>>> ListGroupMembers(string groupId);
}