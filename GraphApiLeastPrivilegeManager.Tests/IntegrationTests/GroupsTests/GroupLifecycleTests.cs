using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace GraphApiLeastPrivilegeManagement.Tests.IntegrationTests.GroupsTests;

public partial class GroupsTests
{
    [Fact]
    public async Task Test_group_create_GetGroupById_destroy()
    {
        /* Arrange */
        GroupEntity? group = default;
        var displayName = $"test-group-{Guid.NewGuid()}";
        var owner = new AzurePrincipal()
        {
            Id = AdminPrincipal.Id,
            DisplayName = AdminPrincipal.DisplayName,
            PrincipalType = PrincipalType.ServicePrincipal
        };
        try
        {

            /* Act and Assert */
            var groupResponse = await _groupService.CreateGroup(owner, displayName, _groupDescription);
            group = groupResponse.Body;
            Assert.NotNull(group);
            await Task.Delay(_throttle * 1000);

            groupResponse = await _groupService.GetGroupById(group.Id);
            group = groupResponse.Body;
            Assert.NotNull(group);
            Assert.Equal(displayName, group.DisplayName);
            CollectionAssert.AreEqual(_groupDescription, group.Description);
        }
        finally
        {
            if (group?.Id != default)
            {
                await Task.Delay(_throttle * 1000);
                await _groupService.DeleteGroup(owner, group.Id);
            }
        }
    }

    [Fact]
    public async Task Test_group_create_GetGroupByDisplayName_destroy()
    {
        /* Arrange */
        GroupEntity? group = default;
        var displayName = $"test-group-{Guid.NewGuid()}";
        var owner = new AzurePrincipal()
        {
            Id = AdminPrincipal.Id,
            DisplayName = AdminPrincipal.DisplayName,
            PrincipalType = PrincipalType.ServicePrincipal
        };
        try
        {

            /* Act and Assert */
            var groupResponse = await _groupService.CreateGroup(owner, displayName, _groupDescription);
            group = groupResponse.Body;
            Assert.NotNull(group);
            await Task.Delay(_throttle * 1000);

            var queryGroupResponse = await _groupService.GetGroupByDisplayName(displayName);
            var queriedGroup = queryGroupResponse.Body;
            Assert.NotNull(queriedGroup);
            Assert.Equal(group.Id, queriedGroup.Id);
            CollectionAssert.AreEqual(_groupDescription, queriedGroup.Description);
        }
        finally
        {
            if (group?.Id != default)
            {
                await Task.Delay(_throttle * 1000);
                await _groupService.DeleteGroup(owner, group.Id);
            }
        }
    }
    
    
    // [Fact]
    // public async Task Test_group_update()
    // {
    //     /* Arrange */
    //     string? groupId = default;
    //     var uuid = Guid.NewGuid();
    //     var displayName = $"test-group-{uuid}";
    //     var newDisplayName = $"updated-test-group-{uuid}";
    //     var newDescription = new Dictionary<string, string>() { { "updated", "value" } };
    //     try
    //     {
    //
    //         /* Act and Assert */
    //         groupId = await _groupService.CreateGroup(displayName, _groupDescription);
    //         Assert.NotNull(groupId);
    //         await Task.Delay(_throttle * 1000);
    //
    //         await _groupService.UpdateGroup(groupId, newDisplayName, newDescription);
    //         await Task.Delay(_throttle * 1000);
    //
    //         var group = await _groupService.GetGroupById(groupId); 
    //         Assert.NotNull(group);
    //         Assert.Equal(groupId, group.Id);
    //         Assert.Equal(newDisplayName, group.DisplayName);
    //         CollectionAssert.AreEqual(newDescription, group.Description);
    //     }
    //     finally
    //     {
    //         if (groupId != default)
    //         {
    //             await Task.Delay(_throttle * 1000);
    //             await _groupService.DeleteGroup(groupId);
    //         }
    //     }
    // }
}