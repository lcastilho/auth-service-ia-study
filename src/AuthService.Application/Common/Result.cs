namespace AuthService.Application.Common;

public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }
}

public sealed class Result<T> : Result
{
    private Result(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, null);
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T>(default, false, error);
    }
}
