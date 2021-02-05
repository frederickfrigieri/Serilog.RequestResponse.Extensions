using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.RequestResponse.Extensions;
using Serilog.RequestResponse.Extensions.Models;
using Serilog.RequestResponseExtension.Extensions;
using Serilog.RequestResponseExtension.Models;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var esConfig = configuration.GetSection("Serilog:Elasticsearch").Get<SerilogElasticsearchConfig>();
            Log.Logger = new LoggerConfiguration()
                .CreateDefaultInstance("APISerilog")
                .WithES(esConfig)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterFilterException();
            services.RegisterLogRequestResponseService();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.RegisterLogRequestResponseMiddleware(new SerilogOptions { UseFilterException = true });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
