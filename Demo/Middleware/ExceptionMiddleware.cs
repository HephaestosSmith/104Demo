using Demo.ErrorHandle;
using Demo.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IO;
using NLog;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Demo.Middlewares
{
    /// <summary>
    /// 錯誤處理中介層
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private const int ReadChunkBufferLength = 4096;
        private readonly NLog.Logger _requestLogger;

        /// <summary>
        /// 初始化 ExceptionMiddleware 類別的實例（DI）
        /// </summary>
        /// <param name="next">RequestDelegate</param>
        /// <param name="configuration">IConfiguration</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        public ExceptionMiddleware(RequestDelegate next,
                                   IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _configuration = configuration;
            _requestLogger = LogManager.GetLogger("requestLog");
        }

        /// <summary>
        /// Pipeline invoke method
        /// </summary>
        /// <param name="httpContext">Http context object</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var config = AppsettingSetting.GetAppsettingConfig(_configuration);

            //控制是否紀錄Requset
            var logRequestFlag = config.LogOptions!.RequestEnabled;
            var logFlag = config.LogOptions.LogEnabled;
            var requestText = string.Empty;
            var responseText = string.Empty;
            try
            {
                //如要記錄Request先行產生Requset內容，Catch之後取不到Requset內容
                if (logRequestFlag && logFlag)
                    requestText = await LogRequest(httpContext);
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var innerEx = string.Empty;
                if (ex.InnerException != null)
                    innerEx = ex.InnerException.Message;

                _logger.LogError(ex, $"================== {ex.Message}, {innerEx}, {ex.StackTrace} ======================");

                // Catch all unexpected exception here
                await HandleExceptionn(httpContext, ex);
                if (logFlag)
                    await LogException(httpContext, ex, logRequestFlag, requestText);
            }
        }

        private static async Task HandleExceptionn(HttpContext context, Exception exception)
        {
            var defaultException = exception;
            HttpStatusCode httpStatus;
            string errorCode = string.Empty;
            var detailMsg = string.Empty;

            try
            {
                if (exception is BaseException baseException)
                {
                    errorCode = baseException.GetErrorCode();
                }
                else
                {
                    var internalException = new InternalException(SystemError.UnknowError, exception.Message);
                    errorCode = internalException.GetErrorCode();

                    // api 如不需回覆詳細錯誤資訊，這裡可以移除
                    var msg = exception.Message;
                    if (exception.InnerException != null)
                        msg += ", " + exception.InnerException.Message;
                    msg += ", " + exception.StackTrace;

                    detailMsg = msg;
                }
            }
            catch (Exception ex)
            {
                defaultException = new InternalException(SystemError.EnumSettingError,
                    $"{ex.Message} Occured when parsing exception: {exception.Message}");

                if (defaultException != null && defaultException is BaseException baseException)
                    errorCode = baseException.GetErrorCode();
            }

            defaultException ??= new InternalException(SystemError.UnknowError);
            httpStatus = exception switch
            {
                BusinessException => HttpStatusCode.BadRequest,
                AuthException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError,
            };

            var responseMessage = new ProblemDetails
            {
                Title = $"Occurred an {defaultException.GetType().Name}",
                Detail = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development") ?? false
                    ? $"[{errorCode}]{defaultException.Message}, {detailMsg}"
                    : $"[{errorCode}]{defaultException.Message}",
                Status = (int)httpStatus,
            };

            await context.Response
                .WriteAsJsonAsync(responseMessage);
        }

        private async Task<string> LogRequest(HttpContext context)
        {
            var result = string.Empty;
            context.Request.EnableBuffering();

            // avoid LOH(Large Object Heap)
            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                var requestBodyText = ReadStreamInChunks(requestStream);

                #region request log

                var path = $"{context.Request.Path}{context.Request.QueryString.Value}";
                var sbHeaders = new StringBuilder();
                foreach (var header in context.Request.Headers)
                {
                    sbHeaders.AppendLine($"{header.Key}: {header.Value}");
                }

                var message = $"Request Method：{context.Request.Method} {Environment.NewLine}" +
                              $"{path}  {context.Request.Protocol}{Environment.NewLine}" +
                              $"{sbHeaders}" +
                              $"--------------Body------------------{Environment.NewLine}" +
                              $"{requestBodyText}{Environment.NewLine}" +
                              $"--------------Request End-----------{Environment.NewLine}";

                var requestParam = requestBodyText;

                try
                {
                    var clientIp = context.Connection.RemoteIpAddress;
                    result = $"ClientIp:{clientIp}{Environment.NewLine}{message}";
                }
                catch (Exception)
                {
                    result = $"api request log failed";
                }

                #endregion request log

                context.Request.Body.Position = 0;

                //Request加密並回傳
                return result;
            }
        }

        private string ReadStreamInChunks(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;
            using (var textWriter = new StringWriter())
            {
                using (var reader = new StreamReader(stream))
                {
                    var readChunk = new char[ReadChunkBufferLength];
                    int readChunkLength;
                    //do while: is useful for the last iteration in case readChunkLength < chunkLength
                    do
                    {
                        readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                        textWriter.Write(readChunk, 0, readChunkLength);
                    } while (readChunkLength > 0);

                    result = textWriter.ToString();
                }
            }

            return result;
        }


        private Task LogException(HttpContext httpContext, Exception ex, bool logRequestFlag, string requestText)
        {
            //儲存需要紀錄的內容
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            var msg = $"{Environment.NewLine}" +
                      $"============{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff")}=============={Environment.NewLine}" +
                      $"TraceId：{traceId}{Environment.NewLine}" +
                      $"StatusCode：{httpContext.Response.StatusCode}{Environment.NewLine}"; ;
            var innerEx = string.Empty;
            if (ex.InnerException != null)
                innerEx = ex.InnerException.Message;

            //顯示錯誤訊息與API路徑以及詳細內容
            msg += $"{httpContext.Request.Path}：{ex.Message}, {innerEx}, {ex.StackTrace}{Environment.NewLine}";

            //如不紀錄Request不顯示
            if (logRequestFlag)
            {
                msg += $"--------------Request Header----------------------{Environment.NewLine}" +
                       $"{requestText}{Environment.NewLine}";
            }

            //產生分隔線易辨識
            msg += $"==================================================";

            //寫到Log
            _requestLogger.Error(msg);

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// 將 <see cref="ExceptionMiddleware"/> 加入到中介層
        /// </summary>
        /// <param name="builder">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder instance</returns>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
