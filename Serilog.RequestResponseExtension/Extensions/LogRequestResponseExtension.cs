using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.RequestResponse.Extensions.Models;
using Serilog.RequestResponseExtension.Middleware;

namespace Serilog.RequestResponseExtension.Extensions
{
    public static class LogRequestResponseExtension
    {
        public static void RegisterLogRequestResponseMiddleware(this IApplicationBuilder app, SerilogOptions options)
        {
            app.UseMiddleware<LogRequestResponseMiddleware>(options);
        }

        public static void RegisterLogRequestResponseService(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });
        }

    }
}
