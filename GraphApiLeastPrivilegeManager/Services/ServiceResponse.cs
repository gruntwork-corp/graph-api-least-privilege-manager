namespace GraphApiLeastPrivilegeManager.Services;

public class ServiceResponse<T>
{
    public string Message { get; init; } = "Success";
    public bool Success { get; init; } = true;
    public int StatusCode { get; init; } = 200;
    public T? Body { get; init; } = default!;
}

public static class ServiceResponseGenerator
{
    public static ServiceResponse<T?> Success<T>(T? body = default, int statusCode = 200, string message = "Success")
    {
        return new ServiceResponse<T?>
        {
            Message = message,
            Success = true,
            StatusCode = statusCode,
            Body = body
        };
    }

    public static ServiceResponse<T?> BadRequest<T>(string message = "Entity does not exist", int statusCode = 400)
    {
        return new ServiceResponse<T?>
        {
            Message = message,
            Success = false,
            StatusCode = statusCode,
            Body = default
        };
    }

    public static ServiceResponse<T?> Forbidden<T>(string message = "Principal not owner of entity", int statusCode = 403)
    {
        return new ServiceResponse<T?>
        {
            Message = message,
            Success = false,
            StatusCode = statusCode,
            Body = default
        };
    }

    public static ServiceResponse<T?> InternalServerError<T>(string message, int statusCode = 500)
    {
        return new ServiceResponse<T?>
        {
            Message = message,
            Success = false,
            StatusCode = statusCode,
            Body = default
        };
    }
}