#pragma warning disable CA1000
namespace ExpenseHub.Api.Services;

/// <summary>Encapsulates the outcome of a service operation including HTTP status context.</summary>
/// <typeparam name="T">The type of the operation result value.</typeparam>
public sealed class ServiceResult<T>
{
    private ServiceResult(T? value, int statusCode, string? error)
    {
        Value = value;
        StatusCode = statusCode;
        Error = error;
    }

    /// <summary>Gets the result value when the operation succeeded.</summary>
    public T? Value { get; }

    /// <summary>Gets the HTTP status code representing the outcome.</summary>
    public int StatusCode { get; }

    /// <summary>Gets the error message when the operation failed.</summary>
    public string? Error { get; }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess => StatusCode is >= 200 and < 300;

    /// <summary>Creates a successful result with HTTP 200.</summary>
    public static ServiceResult<T> Ok(T value) => new(value, 200, null);

    /// <summary>Creates a successful result with HTTP 201.</summary>
    public static ServiceResult<T> Created(T value) => new(value, 201, null);

    /// <summary>Creates a not-found result with HTTP 404.</summary>
    public static ServiceResult<T> NotFound(string error) => new(default, 404, error);

    /// <summary>Creates a forbidden result with HTTP 403.</summary>
    public static ServiceResult<T> Forbidden(string error) => new(default, 403, error);

    /// <summary>Creates a conflict result with HTTP 409.</summary>
    public static ServiceResult<T> Conflict(string error) => new(default, 409, error);

    /// <summary>Creates a bad-request result with HTTP 400.</summary>
    public static ServiceResult<T> BadRequest(string error) => new(default, 400, error);
}
