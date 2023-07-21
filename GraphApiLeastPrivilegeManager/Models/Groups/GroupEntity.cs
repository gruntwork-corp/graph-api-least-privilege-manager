namespace GraphApiLeastPrivilegeManager.Models.Groups;

public class GroupEntity
{
    public string Id { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public Dictionary<string, string>? Description { get; init; }
}