using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

[assembly: Convention(typeof(NewtonsoftJsonConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    public class NewtonsoftJsonConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services
               .AddOptions<JsonSerializerSettings>()
               .Configure<IDateTimeZoneProvider>(
                    (options, provider) => options.ConfigureForLaunchPad(provider)
                );
        }
    }
}