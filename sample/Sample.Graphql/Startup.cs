using AutoMapper.QueryableExtensions;
using HotChocolate.Execution;
using HotChocolate.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.EntityFramework;
using Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Sample.Graphql;
using Serilog;
using System.Linq.Expressions;
using RequestDelegate = Microsoft.AspNetCore.Http.RequestDelegate;

namespace Sample.Graphql
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureGraphqlRootType, AutoConfigureDbContextConfigureQueryType<RocketDbContext>>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDbContextConfigureQueryEntity, ConfigureReadyRocketQueryType>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDbContextConfigureQueryEntity, ConfigureLaunchRecordQueryType>());

            services
               .AddGraphQLServer()
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