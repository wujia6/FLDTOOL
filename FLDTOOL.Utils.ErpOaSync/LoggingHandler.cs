using System.Text;
using Microsoft.Extensions.Logging;

namespace FLDTOOL.Utils.ErpOaSync
{
    public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger = logger;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 记录请求
            var requestLog = await FormatRequestAsync(request);
            _logger.LogInformation("HTTP Request:\n{RequestLog}", requestLog);

            // 发送请求并获取响应
            var response = await base.SendAsync(request, cancellationToken);

            // 记录响应
            var responseLog = await FormatResponseAsync(response);
            _logger.LogInformation("HTTP Response:\n{ResponseLog}", responseLog);

            return response;
        }

        private static async Task<string> FormatRequestAsync(HttpRequestMessage request)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{request.Method} {request.RequestUri} HTTP/{request.Version}");

            // 请求头
            foreach (var header in request.Headers)
            {
                sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                // 请求体
                var requestBody = await request.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    sb.AppendLine();
                    sb.AppendLine(requestBody);
                }
            }

            return sb.ToString();
        }

        private static async Task<string> FormatResponseAsync(HttpResponseMessage response)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.StatusCode}");

            // 响应头
            foreach (var header in response.Headers)
            {
                sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                // 响应体
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseBody))
                {
                    sb.AppendLine();
                    sb.AppendLine(responseBody);
                }
            }

            return sb.ToString();
        }
    }
}

