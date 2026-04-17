namespace BLL.Common
{
    public enum ServiceErrorCode
    {
        None,
        ValidationError,      // 400
        NotFound,             // 404
        UpstreamServiceError,  // 503
        Unauthorized           // 401

    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public ServiceErrorCode ErrorCode { get; private set; } = ServiceErrorCode.None;

        public static ServiceResult<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static ServiceResult<T> Failure(string error, ServiceErrorCode code = ServiceErrorCode.ValidationError) =>
            new() { IsSuccess = false, ErrorMessage = error, ErrorCode = code };
    }
}