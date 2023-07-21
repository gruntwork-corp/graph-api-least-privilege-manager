using System.Text.Json.Serialization;

namespace GraphApiLeastPrivilegeManager.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrincipalType
{
    [JsonPropertyName("user")] User,
    [JsonPropertyName("servicePrincipal")] ServicePrincipal,

}

public static class PrincipalTypeExtension
{
    public static string ToString(this PrincipalType value)
    {
        switch (value)
        {
            case PrincipalType.User: return "user";
            case PrincipalType.ServicePrincipal: return "servicePrincipal";
            default: throw new NotImplementedException();
        }
    }

    public static string ToPluralString(this PrincipalType value)
    {
        switch (value)
        {
            case PrincipalType.User: return "users";
            case PrincipalType.ServicePrincipal: return "servicePrincipals";
            default: throw new NotImplementedException();
        }
    }
}