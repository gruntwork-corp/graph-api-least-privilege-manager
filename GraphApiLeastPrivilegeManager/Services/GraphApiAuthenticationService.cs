using Azure.Core;
using Azure.Identity;

namespace GraphApiLeastPrivilegeManager.Services;

public class GraphApiAuthenticationService : IGraphApiAuthenticationService
{
    private readonly TokenCredential _clientSecretCredential;
    private readonly string[] _scopes;

    /// <summary>
    /// The client credentials flow requires that you request the
    /// /.default scope, and pre-configure your permissions on the
    /// app registration in Azure. An administrator must grant consent
    /// to those permissions beforehand.
    /// </summary>
    public GraphApiAuthenticationService(string tenantId, string clientId, string clientSecret)
    {
        _scopes = new[] { "https://graph.microsoft.com/.default" };
        _clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret);
    }

    public TokenCredential GetCredentials()
    {
        return _clientSecretCredential;
    }

    public string[]? GetScopes()
    {
        return _scopes;
    }
}
