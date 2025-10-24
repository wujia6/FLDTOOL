using System.Net;
using Newtonsoft.Json;

namespace FLDTOOL.Api.FullantiOaSync.Middleware
{
    /// <summary>
    /// 全局异常中间件
    /// </summary>
    /// <remarks>
    /// ioc
    /// </remarks>
    /// <param name="next"></param>
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //context.Response.ContentType = "application/json";
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //var response = new
            //{
            //    success = false,
            //    statusCode = context.Response.StatusCode,
            //    message = "内部错误", // 可以自定义错误消息
            //    detailed = exception.Message // 可选，详细信息（注意不要在生产环境中泄露详细信息）
            //};
            //return context.Response.WriteAsync(JsonConvert.SerializeObject(response));

            // 这里根据异常类型可以细化处理，示例只做统一处理
            var code = HttpStatusCode.InternalServerError; // 500 默认错误

            // 你可以逐个处理不同异常类型，修改 code 和 message
            if (exception is ArgumentException)
                code = HttpStatusCode.BadRequest;

            var response = new
            {
                StatusCode = (int)code,
                exception.Message,
                Data = (object)null
            };

            var result = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }
}
