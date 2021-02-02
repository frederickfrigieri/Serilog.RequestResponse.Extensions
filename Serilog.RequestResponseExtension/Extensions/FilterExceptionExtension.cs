using Microsoft.Extensions.DependencyInjection;
using Serilog.RequestResponse.Extensions.Filters;

namespace Serilog.RequestResponse.Extensions
{
    public static class FilterExceptionExtension
    {
        public static void RegisterFilterException(this IServiceCollection service)
        {
            service.AddMvc(x => x.Filters.Add(new FilterException()));
        }
    }
}
