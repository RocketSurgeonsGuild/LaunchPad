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
        public void Register(IServiceConventionContext context)
        {
            context.Services.AddSingleton<IOnModelCreating, SqliteDateTimeOffset>();
            context.Services.AddSingleton<IOnModelCreating, ConfigureEntityTypes>();

            var assemblies = context.AssemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.LaunchPad.EntityFramework");
            context.Services.Scan(
                x => x
                   .FromAssemblies(assemblies)
                   .AddClasses(c => c.AssignableTo<IOnEntityCreating>())
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime()
            );
        }
    }
}