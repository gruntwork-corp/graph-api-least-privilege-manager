using GraphApiLeastPrivilegeManagement.Tests.Fixtures;
using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Services.Groups;

namespace GraphApiLeastPrivilegeManagement.Tests.IntegrationTests.GroupsTests;

public partial class GroupsTests: IAssemblyFixture<AdminFixture>
{
    private readonly GraphApiGroupService _groupService;
    private readonly AzurePrincipal AdminPrincipal;
    private readonly Dictionary<string, string> _groupDescription;
    private readonly int _throttle;

    public GroupsTests(AdminFixture fixture)
    {
        var groupFixture = new GroupFixture(fixture);
        _groupService = groupFixture.GroupService;
        AdminPrincipal = groupFixture.AdminPrincipal;
        _groupDescription = new Dictionary<string, string>
        {
            {"created-by", "GraphApiLeastPrivilegeManger.Tests/GroupsTests"},
            {"created", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}"},
            {"delete-me", "true"}

        };
        _throttle = 5; //5 seconds between API operations
    }
}