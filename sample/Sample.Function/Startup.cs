using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Functions;
using Sample_Function;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Sample_Function
{
    public class Startup : LaunchPadFunctionStartup
    {
        public override void Setup(ConventionContextBuilder contextBuilder)
        {

        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder, IConventionContext context)
        {

        }

        public override void Configure(IFunctionsHostBuilder builder, IConventionContext context)
        {

        }
    }
}