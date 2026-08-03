using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Services
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public ErrorType ErrorType { get; init; }

        protected Result(bool isSuccess, string? error, ErrorType errorType = ErrorType.None)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success() => new(true, null, ErrorType.None);

        public static Result Failure(string error, ErrorType errorType) => new(false, error, errorType);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value)
            : base(true, null, ErrorType.None)
        {
            Value = value;
        }

        private Result(string error, ErrorType errorType)
            : base(false, error, errorType)
        {
        }

        public static Result<T> Success(T value) => new(value);
        public new static Result<T> Failure(string error, ErrorType errorType) => new(error, errorType);
    }
}
