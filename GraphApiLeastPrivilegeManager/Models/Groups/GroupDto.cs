namespace GraphApiLeastPrivilegeManager.Models.Groups;

public class GroupDto
{
    public string DisplayName { get; init; } = null!;
    public Dictionary<string, string> Description { get; init; } = null!;
}