namespace HomeSmartLink.Application.Common.Models;

public class ApiResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public int? StatusCode { get; init; }

    public static ApiResult Success() => new() { IsSuccess = true };
    public static ApiResult Failure(string errorMessage, int? statusCode = null) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }

    public static ApiResult<T> Success(T data) => new() { IsSuccess = true, Data = data };

    public new static ApiResult<T> Failure(string errorMessage, int? statusCode = null) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
}
