using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.RequestResponseExtension.Middleware;

namespace Serilog.RequestResponseExtension.Extensions
{
    public static class LogRequestResponseExtension
    {
        public static void RegisterLogRequestResponseMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<LogRequestResponseMiddleware>();
        }

        public static void RegisterLogRequestResponseService(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(dispose: true); });
        }

    }
}
