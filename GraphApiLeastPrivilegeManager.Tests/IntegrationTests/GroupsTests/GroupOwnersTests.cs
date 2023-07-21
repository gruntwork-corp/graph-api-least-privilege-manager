using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace GraphApiLeastPrivilegeManagement.Tests.IntegrationTests.GroupsTests;

public partial class GroupsTests
{
    [Fact]
    public async Task Test_create_group()
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
        var owners = new List<AzurePrincipal> { owner };
        try
        {
            /* Act and Assert */
            var response = await _groupService.CreateGroup(owner, displayName, _groupDescription);
            Assert.NotNull(response);
            group = response.Body;
            Assert.NotNull(group);
            await Task.Delay(_throttle * 1000);

            var groupOwners = await _groupService.ListGroupOwners(owner, group.Id);

            Assert.Single(groupOwners.Body!);
            CollectionAssert.AreEqual(owners, groupOwners.Body);
        }
        finally
        {
            if (group != default)
            {
                await Task.Delay(_throttle * 1000);
                await _groupService.DeleteGroup(owner, group.Id);
            }
        }
    }

    // [Fact]
    // public async Task Test_create_group_and_add_owner()
    // {
    //     /* Arrange */
    //     string? groupId = default;
    //     var displayName = $"test-group-{Guid.NewGuid()}";
    //     var owner = new AzurePrincipal
    //     {
    //         Id = AdminPrincipal.Id,
    //         DisplayName = AdminPrincipal.DisplayName,
    //         PrincipalType = PrincipalType.ServicePrincipal
    //     };
    //     try
    //     {
    //         /* Act and Assert */
    //         groupId = await _groupService.CreateGroup(displayName, _groupDescription);
    //         Assert.NotNull(groupId);
    //         await _groupService.AddGroupOwner(groupId, owner);
    //
    //         await Task.Delay(_throttle * 1000);
    //         var groupOwners = await _groupService.ListGroupOwners(groupId);
    //
    //         Assert.Single(groupOwners);
    //         Assert.Equal(owner, groupOwners.FirstOrDefault());
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

    // [Fact]
    // public async Task Test_create_group_with_owner_and_remove_owner()
    // {
    //     /* Arrange */
    //     GroupEntity? group = default;
    //     var displayName = $"test-group-{Guid.NewGuid()}";
    //     var owner = new AzurePrincipal
    //     {
    //         Id = AdminPrincipal.Id,
    //         DisplayName = AdminPrincipal.DisplayName,
    //         PrincipalType = PrincipalType.ServicePrincipal
    //     };
    //     try
    //     {
    //         /* Act and Assert */
    //         var groupResponse = await _groupService.CreateGroup(displayName, _groupDescription, owner);
    //         Assert.NotNull(groupResponse);
    //         group = groupResponse.Body;
    //         Assert.NotNull(group);
    //
    //         await Task.Delay(_throttle * 1000);
    //         var groupOwnersResponse = await _groupService.ListGroupOwners(owner, group.Id);
    //         var groupOwners = groupOwnersResponse.Body!;
    //         Assert.Single(groupOwners);
    //         Assert.Equal(owner, groupOwners.FirstOrDefault());
    //
    //         await _groupService.RemoveGroupOwner(owner, group.Id, owner);
    //         await Task.Delay(_throttle * 1000);
    //         groupOwnersResponse = await _groupService.ListGroupOwners(owner, group.Id);
    //         groupOwners = groupOwnersResponse.Body!;
    //         Assert.Empty(groupOwners);
    //     }
    //     finally
    //     {
    //         if (group?.Id != default)
    //         {
    //             await Task.Delay(_throttle * 1000);
    //             await _groupService.DeleteGroup(owner, group.Id);
    //         }
    //     }
    // }
    //
    // [Fact]
    // public async Task Test_is_group_owner()
    // {
    //     /* Arrange */
    //     GroupEntity? group = default;
    //     var displayName = $"test-group-{Guid.NewGuid()}";
    //     var owner = new AzurePrincipal
    //     {
    //         Id = AdminPrincipal.Id,
    //         DisplayName = AdminPrincipal.DisplayName,
    //         PrincipalType = PrincipalType.ServicePrincipal
    //     };
    //     try
    //     {
    //         /* Act and Assert */
    //         var groupResponse = await _groupService.CreateGroup(displayName, _groupDescription, owner);
    //         Assert.NotNull(groupResponse);
    //         group = groupResponse.Body;
    //         Assert.NotNull(group);
    //
    //         await Task.Delay(_throttle * 1000);
    //         var isOwnerResponse = await _groupService.IsPrincipalGroupOwner(owner, group.Id, owner);
    //         Assert.True(isOwnerResponse.Body);
    //
    //         await _groupService.RemoveGroupOwner(owner, group.Id, owner);
    //         await Task.Delay(_throttle * 1000);
    //         isOwnerResponse = await _groupService.IsPrincipalGroupOwner(owner, group.Id, owner);
    //         Assert.False(isOwnerResponse.Body);
    //     }
    //     finally
    //     {
    //         if (group?.Id != default)
    //         {
    //             await Task.Delay(_throttle * 1000);
    //             await _groupService.DeleteGroup(owner, group.Id);
    //         }
    //     }
    // }
}