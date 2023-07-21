using Azure.Core;
using GraphApiLeastPrivilegeManager.Services;

namespace GraphApiLeastPrivilegeManagement.Tests.Utilities.Mocks;

public class MockGraphApiAuthenticationService : IGraphApiAuthenticationService
{
    public TokenCredential GetCredentials()
    {
        return default!;
    }

    public string[]? GetScopes()
    {
        return Array.Empty<string>();
    }
}