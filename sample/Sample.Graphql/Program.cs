using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.HotChocolate;

namespace Sample.Graphql;

[ImportConventions]
public static partial class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .LaunchWith(
                        RocketBooster.ForDependencyContext(DependencyContext.Default),
                        z => z
                            .WithConventionsFrom(GetConventions)
                            .Set(
                                 new RocketChocolateOptions
                                 {
                                     RequestPredicate = type =>
                                         type is { IsNested: true, DeclaringType: { } }
                                      && !( type.Name.StartsWith("Get", StringComparison.Ordinal)
                                         || type.Name.StartsWith(
                                                "List", StringComparison.Ordinal
                                            ) ),
                                     IncludeAssemblyInfoQuery = true
                                 }
                             )
                    )
                   .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
