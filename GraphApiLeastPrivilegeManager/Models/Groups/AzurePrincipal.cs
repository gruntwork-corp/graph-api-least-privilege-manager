using GraphApiLeastPrivilegeManager.Models.Enums;

namespace GraphApiLeastPrivilegeManager.Models.Groups;

public class AzurePrincipal
{
    public string Id { get; init; } = null!;
    public string? DisplayName { get; init; }//TODO: remove this
    public PrincipalType PrincipalType { get; init; }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as AzurePrincipal);
    }

    public bool Equals(AzurePrincipal? other)
    {
        return other != null &&
               Id == other.Id &&
               PrincipalType == other.PrincipalType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, PrincipalType);
    }

    public static bool operator ==(AzurePrincipal? left, AzurePrincipal? right)
    {
        return EqualityComparer<AzurePrincipal>.Default.Equals(left, right);
    }

    public static bool operator !=(AzurePrincipal? left, AzurePrincipal? right)
    {
        return !(left == right);
    }
}