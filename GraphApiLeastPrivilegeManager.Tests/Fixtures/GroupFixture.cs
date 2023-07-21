using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Services.Groups;
using Microsoft.Extensions.Logging;
using Moq;

namespace GraphApiLeastPrivilegeManagement.Tests.Fixtures;

public class GroupFixture : IDisposable
{
    public readonly AzurePrincipal AdminPrincipal;
    public readonly GraphApiGroupService GroupService;
    

    public GroupFixture(AdminFixture fixture)
    {
        var logger = new Mock<ILogger<GraphApiGroupService>>();
        AdminPrincipal = fixture.AdminPrincipal;
        GroupService = new GraphApiGroupService(logger.Object, fixture.GraphCli, fixture.BetaGraphCli);
    }

    public void Dispose() { }
}