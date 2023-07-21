using System.Security.Claims;
using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;

namespace GraphApiLeastPrivilegeManager.Utilities;

public class PrincipalUtils
{
    public static AzurePrincipal? ExtractPrincipal(ClaimsPrincipal user)
    {
        var oid = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        var userName = user.FindFirst("name")?.Value;
        var applicationName = user.FindFirst("app_displayname")?.Value;
        if (userName is null && applicationName is null) { return null;}
        return new AzurePrincipal
        {
            Id = oid!,
            DisplayName = userName ?? applicationName,
            PrincipalType = userName is not null ? PrincipalType.User : PrincipalType.ServicePrincipal
        };
    }
}