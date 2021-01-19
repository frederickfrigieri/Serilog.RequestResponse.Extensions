using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.RequestResponseExtension.Middleware
{
    public class LogRequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        private string _requestBody;
        private string _responseBody;
        private string _requestQueryString;
        private Stopwatch _stopWatch;

        public LogRequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            _stopWatch = new Stopwatch();
            await ProcessRequest(context);
            await ProcessResponse(context);

            Log.ForContext("RequestBody", _requestBody)
                .ForContext("RequestHeaders", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestQueryString", _requestQueryString)
                .ForContext("ResponseBody", _responseBody)
                .ForContext("TimeResponse", _stopWatch.ElapsedMilliseconds)
                .ForContext("UsuarioId", context.User?.Identity.Name)
                .Information("Response information {RequestMethod} {RequestPath} {statusCode}",
                  context.Request.Method, context.Request.Path, context.Response.StatusCode);
        }

        private async Task ProcessRequest(HttpContext context)
        {
            _stopWatch.Start();
            context.Request.EnableBuffering();

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                _requestBody = ReadStreamInChunks(requestStream);
                _requestQueryString = context.Request.QueryString.ToString();
                context.Request.Body.Position = 0;
            }
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using (var textWriter = new StringWriter())
            {
                using (var reader = new StreamReader(stream))
                {
                    var readChunk = new char[readChunkBufferLength];
                    int readChunkLength;
                    do
                    {
                        readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                        textWriter.Write(readChunk, 0, readChunkLength);
                    } while (readChunkLength > 0);

                    return textWriter.ToString();
                }
            }
        }

        private async Task ProcessResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using (var memoryStream = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = memoryStream;
                await _next(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                _responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }
    }

}
