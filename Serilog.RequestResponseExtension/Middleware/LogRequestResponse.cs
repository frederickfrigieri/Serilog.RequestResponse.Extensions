using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog.RequestResponse.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.RequestResponseExtension.Middleware
{
    public class LogRequestResponse
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly LogRequestResponseOptions _options;
        private string _requestBody;
        private string _responseBody;
        private string _requestQueryString;
        private Stopwatch _stopWatch;

        public LogRequestResponse(RequestDelegate next, LogRequestResponseOptions options)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            _stopWatch = new Stopwatch();

            await ProcessRequest(context);

            //Quando não tiver usando um filtro para tratamento de exceção deve logar a requisição sempre no início
            if (!_options.UseFilterOrMiddlewareException)
                LogRequest(context);

            await ProcessResponse(context);

            LogRequestAndResponse(context);
        }

        #region Methods for Middleware
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
        #endregion


        #region Methods for Logger Context
        //Esses métodos poderiam estar em um outro serviço???
        //Pode criar um serviço que recebe um context e loga ele
        //Precisa pegar o body do context e o requeststring sem precisar ser do middleware

        private Dictionary<string, string> ConvertDictionary(IHeaderDictionary headerDictionary) => headerDictionary.ToDictionary(h => h.Key, h => h.Value.ToString());

        private ILogger Logger(HttpContext context)
        {
            var logger = Log.ForContext("RequestBody", _requestBody)
                            .ForContext("RequestHeaders", ConvertDictionary(context.Request.Headers), destructureObjects: true)
                            .ForContext("RequestQueryString", _requestQueryString)
                            .ForContext("UsuarioId", context.User?.Identity.Name);
            return logger;
        }

        private void LogRequest(HttpContext context)
        {
            if (_options.NotShouldLog(context, TypeRemoveHttpLog.Method))
                return;

            Logger(context).Information("Response information {RequestMethod} {RequestPath}", context.Request.Method, context.Request.Path);
        }

        private void LogRequestAndResponse(HttpContext context)
        {
            var error = context.Response.StatusCode >= 400 || context.Items["Exception"] != null;

            if (error && _options.NotShouldLog(context, TypeRemoveHttpLog.StatusCode)) return;
            if (!error && _options.ExistsRuleForNotLog(context)) return;

            Logger(context)
                .ForContext("ResponseBody", _responseBody)
                .ForContext("TimeResponse", _stopWatch.ElapsedMilliseconds)
                .ForContext("Exception", context.Items["Exception"], destructureObjects: true)
                .Information("Response information {RequestMethod} {RequestPath} {statusCode}",
                  context.Request.Method, context.Request.Path, context.Response.StatusCode);
        }
        #endregion
    }

}
