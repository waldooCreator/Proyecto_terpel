namespace Domain.Primitives
{
    public class OperationResult<T>
    {
        public bool Success { get; }
        public string? Message { get; }
        public T? Data { get; }

        private OperationResult(bool success, T? data, string? message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public static OperationResult<T> Ok(T data, string? message = null) => new(true, data, message);

        public static OperationResult<T> Fail(string message) => new(false, default, message);
    }
}
