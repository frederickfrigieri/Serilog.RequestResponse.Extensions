using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.RequestResponseExtension.Middleware
{
    public class LogRequestResponseMiddleware
    {
        private RequestDelegate _next;

        public LogRequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];

            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            var requestBody = Encoding.UTF8.GetString(buffer);
            var requestQuerystring = context.Request.QueryString;

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var originalBodyStream = context.Response.Body;
            var dataInicial = DateTime.Now;

            using (var responseBodyMemoryStream = new MemoryStream())
            {
                context.Response.Body = responseBodyMemoryStream;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await responseBodyMemoryStream.CopyToAsync(originalBodyStream);

                Log.ForContext("RequestBody", requestBody)
                    .ForContext("RequestHeaders", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                    .ForContext("RequestQueryString", requestQuerystring)
                    .ForContext("ResponseBody", responseBody)
                    .ForContext("TimeResponse", (DateTime.Now - dataInicial).TotalMilliseconds)
                    .ForContext("UsuarioId", context.User?.Identity.Name)
                    .Information("Response information {RequestMethod} {RequestPath} {statusCode}",
                      context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }

        }
    }
}
