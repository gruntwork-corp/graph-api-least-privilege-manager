using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GraphApiLeastPrivilegeManagement.Tests.Factories;
using GraphApiLeastPrivilegeManagement.Tests.Utilities;
using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;

namespace GraphApiLeastPrivilegeManagement.Tests.UnitTests.AuthorizationTests;

public class AuthorizationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(ApiFactory fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Test_unauthorized()
    {
        var response = await _client.GetAsync("/authorization/all");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_GroupReadOwnedBy_authorized()
    {
        var token = new TestJwtToken()
            .WithCustom("http://schemas.microsoft.com/identity/claims/objectidentifier", "12345")
            .WithCustom("app_displayname", "TestApplication")
            .WithRole("Group.ReadWrite.OwnedBy")
            .WithExpiration(30)
            .Build();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var principal = await _client.GetFromJsonAsync<AzurePrincipal>("/authorization/all");
        Assert.NotNull(principal);
        Assert.Equal("12345", principal.Id);
        Assert.Equal("TestApplication", principal.DisplayName);
        Assert.Equal(PrincipalType.ServicePrincipal, principal.PrincipalType);
    }
}