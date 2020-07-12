using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Rocket.Surgery.LaunchPad.Functions;
using Sample_Function;


[assembly: WebJobsStartup(typeof(Startup))]
namespace Sample_Function
{
    public class Startup : LaunchPadFunctionStartup
    {
        public override void Setup(IFunctionsHostBuilder builder) { }
    }
}