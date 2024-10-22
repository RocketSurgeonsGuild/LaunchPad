using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

[ExportConvention]
[AfterConvention<SerilogHostingConvention>]
[ConventionCategory(ConventionCategory.Application)]
internal class HostingConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddHostedService<ApplicationLifecycleService>();
    }
}
