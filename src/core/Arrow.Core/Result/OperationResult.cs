namespace Arrow.Core;

/// <summary>C# equivalent of Kotlin <c>Result</c>.</summary>
public readonly struct OperationResult<T>
{
    private OperationResult(bool isSuccess, T value, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        Exception = exception;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value { get; }

    public Exception? Exception { get; }

    public static OperationResult<T> Success(T value) => new(true, value, null);

    public static OperationResult<T> Failure(Exception exception) => new(false, default!, exception);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Exception, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Exception!);
}

public static class OperationResultExtensions
{
    public static OperationResult<B> FlatMap<A, B>(this OperationResult<A> result, Func<A, OperationResult<B>> transform) =>
        result.IsSuccess ? transform(result.Value) : OperationResult<B>.Failure(result.Exception!);
}
