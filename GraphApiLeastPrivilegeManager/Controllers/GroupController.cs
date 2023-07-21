using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Services.Groups;
using GraphApiLeastPrivilegeManager.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GraphApiLeastPrivilegeManager.Controllers;

// [Authorize]
[ApiController]
[Route("[controller]")]
public class GroupController : Controller
{
    private readonly ILogger<GroupController> _logger;
    private readonly IGraphApiGroupService _groupService;

    public GroupController(ILogger<GroupController> logger, IGraphApiGroupService groupService)
    {
        _logger = logger;
        _groupService = groupService;
    }
    [HttpGet("query/{displayName}")]
    [Authorize(Roles = "Group.Read,Group.ReadWriteControl.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(GroupEntity), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult<GroupEntity?>> GetByDisplayName(string displayName)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.GetGroupByDisplayName(displayName);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpGet("{groupId}")]
    [Authorize(Roles = "Group.Read,Group.ReadWriteControl.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(GroupEntity), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult<GroupEntity?>> GetGroupById(string groupId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.GetGroupById(groupId);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpPost]
    [Authorize(Roles = "Group.ReadWriteControl.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(GroupEntity), 200)]
    [ProducesResponseType(typeof(void), 412)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<ActionResult<GroupEntity?>> CreateGroup([FromBody] GroupDto body)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.CreateGroup(owner, body.DisplayName, body.Description);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpPut("{groupId}")]
    [Authorize(Roles = "Group.ReadWriteControl.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(GroupEntity), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult<GroupEntity?>> UpdateGroup(string groupId, [FromBody] GroupDto body)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.UpdateGroup(owner, groupId, body.DisplayName, body.Description);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpDelete("{groupId}")]
    [Authorize(Roles = "Group.ReadWriteControl.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult> DeleteGroup(string groupId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.DeleteGroup(owner, groupId);
        return StatusCode(result.StatusCode);
    }

    [HttpPost("{groupId}/owners/{principalObjectId}")]
    [Authorize(Roles = "Group.ReadWriteOwner.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult> AddGroupOwner(string groupId, string principalObjectId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.AddGroupOwner(owner, groupId, principalObjectId);
        return StatusCode(result.StatusCode);
    }

    [HttpDelete("{groupId}/owners/{principalObjectId}")]
    [Authorize(Roles = "Group.ReadWriteOwner.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult> RemoveGroupOwner(string groupId, string principalObjectId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.RemoveGroupOwner(owner, groupId, principalObjectId);
        return StatusCode(result.StatusCode);
    }

    [HttpGet("{groupId}/owners/{principalObjectId}")]
    [Authorize(Roles = "Group.ReadWriteOwner.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    public async Task<ActionResult<bool>> IsGroupOwner(string groupId, string principalObjectId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.IsPrincipalGroupOwner(owner, groupId, principalObjectId);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpGet("{groupId}/owners")]
    [Authorize(Roles = "Group.ReadWriteOwner.OwnedBy,Group.ReadWrite.OwnedBy")]
    [ProducesResponseType(typeof(List<AzurePrincipal>), 200)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 412)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<ActionResult<List<AzurePrincipal>>> ListGroupOwners(string groupId)
    {
        var owner = PrincipalUtils.ExtractPrincipal(User);
        if (owner is null) { return StatusCode(412); }

        var result = await _groupService.ListGroupOwners(owner, groupId);
        return StatusCode(result.StatusCode, result.Body);
    }

    [HttpPost("{groupId}/members/{principalObjectId}")]
    [Authorize(Roles = "Group.ReadWriteMember.OwnedBy,Group.ReadWrite.OwnedBy")]
    // [ProducesResponseType(typeof(void), 200)]
    // [ProducesResponseType(typeof(void), 400)]
    // [ProducesResponseType(typeof(void), 403)]
    // [ProducesResponseType(typeof(void), 412)]
    [ProducesResponseType(typeof(void), 501)]
    public ActionResult AddGroupMember(string groupId, string principalObjectId)
    {
        var message = $"AddGroupMember GroupId: {groupId}, PrincipalObjectId: {principalObjectId}";
        _logger.LogInformation(message);
        return StatusCode(501);
    }

    [HttpDelete("{groupId}/members/{principalObjectId}")]
    [Authorize(Roles = "Group.ReadWriteMember.OwnedBy,Group.ReadWrite.OwnedBy")]
    // [ProducesResponseType(typeof(void), 200)]
    // [ProducesResponseType(typeof(void), 400)]
    // [ProducesResponseType(typeof(void), 403)]
    // [ProducesResponseType(typeof(void), 412)]
    [ProducesResponseType(typeof(void), 501)]
    public ActionResult RemoveGroupMember(string groupId, string principalObjectId)
    {
        var message = $"RemoveGroupMember GroupId: {groupId}, PrincipalObjectId: {principalObjectId}";
        _logger.LogInformation(message);
        return StatusCode(501);
    }

    [HttpGet("{groupId}/members/{principalObjectId}")]
    [Authorize(Roles = "Group.Read,Group.ReadWrite.OwnedBy")]
    // [ProducesResponseType(typeof(bool), 200)]
    // [ProducesResponseType(typeof(void), 403)]
    // [ProducesResponseType(typeof(void), 412)]
    [ProducesResponseType(typeof(void), 501)]
    public ActionResult<bool> IsGroupMember(string groupId, string principalObjectId)
    {
        var message = $"IsGroupMember GroupId: {groupId}, PrincipalObjectId: {principalObjectId}";
        _logger.LogInformation(message);
        return StatusCode(501);
    }

    [HttpGet("{groupId}/members")]
    [Authorize(Roles = "Group.Read,Group.ReadWriteMember.OwnedBy,Group.ReadWrite.OwnedBy")]
    // [ProducesResponseType(typeof(List<AzurePrincipal>), 200)]
    // [ProducesResponseType(typeof(void), 403)]
    // [ProducesResponseType(typeof(void), 412)]
    // [ProducesResponseType(typeof(void), 500)]
    [ProducesResponseType(typeof(void), 501)]
    public ActionResult<List<AzurePrincipal>> ListGroupMembers(string groupId)
    {
        var message = $"ListGroupMembers GroupId: {groupId}";
        _logger.LogInformation(message);
        return StatusCode(501);
    }
}