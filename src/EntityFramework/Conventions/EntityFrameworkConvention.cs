#if CONVENTIONS
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.EntityFramework.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: Convention(typeof(EntityFrameworkConvention))]

namespace Rocket.Surgery.LaunchPad.EntityFramework.Conventions
{
    public class EntityFrameworkConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services.AddLaunchPadEntityFramework(context.AssemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.LaunchPad.EntityFramework"));
        }
    }
}
#endif