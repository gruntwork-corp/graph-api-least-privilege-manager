using System.Text.Json;

namespace GraphApiLeastPrivilegeManager.Utilities;

public static class JsonUtils
{
    public static T? TryLoadJson<T>(string jsonContent)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(jsonContent);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static T TryLoadJson<T>(string jsonContent, T fallback)
    {
        if (jsonContent is null)
        {
            return fallback;
        }
        try
        {
            var result = JsonSerializer.Deserialize<T>(jsonContent);
            return result ?? fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }
}