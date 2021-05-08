using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;
using Sample.Core.Domain;
using Serilog;

namespace Sample.Graphql
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
               .AddGraphQLServer()
               .AddDefaultTransactionScopeHandler()
               .AddQueryType()
               .AddMutationType()
               .ConfigureSchema(
                    s =>
                    {
                        s.AddType(
                            new ConfigureConfigureEntityFrameworkContextQueryType<RocketDbContext>(
                                new ConfigureReadyRocketType(),
                                new ConfigureLaunchRecordType()
                            )
                        );
                    })
               .AddSorting()
               .AddFiltering()
               .AddProjections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Should this move into an extension method?
            app.UseSerilogRequestLogging(
                x =>
                {
                    x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
                    x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
                }
            );
            app.UseMetricsAllMiddleware();

            app.UseRouting();

            app.UseEndpoints(
                endpoints => { endpoints.MapGraphQL(); }
            );
        }
    }
}