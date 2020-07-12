using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample_Function;

[assembly: Convention(typeof(LocalServices))]

namespace Sample_Function
{
    public class LocalServices : IServiceConvention
    {
        public void Register(IServiceConventionContext context)
        {
            context.Services.AddSingleton(new Service());
        }
    }
}