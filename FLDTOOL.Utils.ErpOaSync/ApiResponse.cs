namespace FLDTOOL.Utils.ErpOaSync
{
    public class ApiResponse<T> where T : class
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data = default, string message = "成功") => new() 
        {
            Data = data,
            Message = message,
            StatusCode = 200
        };

        public static ApiResponse<T> Fail(string message, int statusCode = 400) => new()
        {
            StatusCode = statusCode,
            Message = message,
            Data = default
        };
    }
}
