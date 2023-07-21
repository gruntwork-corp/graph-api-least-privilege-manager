using Azure.Core;

namespace GraphApiLeastPrivilegeManager.Services;

public interface IGraphApiAuthenticationService
{
    public TokenCredential GetCredentials();
    public string[]? GetScopes();
}