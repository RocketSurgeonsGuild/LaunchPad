using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;
using DryIoc;

namespace Sample.Command
{
    public class Program
    {
        public static Task<int> Main(string[] args) => CreateHostBuilder(args).RunCli();

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default))
           .ConfigureRocketSurgery(
                builder => builder
                   .UseDryIoc()
                   .AppendDelegate(new DryIocConventionDelegate(
                        x => x.ConfigureContainer(x => x.UseInstance(new InstanceThing()))
                    ))
                   .ConfigureCommandLine(
                        context =>
                        {
                            context.OnRun<Default>();
                            context.AddCommand<Dump>();
                        }
                    )
            );
    }

    public class InstanceThing
    {
        public string From = "DryIoc";
    }

    [Command("dump")]
    public class Dump
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dump> _logger;
        private readonly InstanceThing _instanceThing;

        public Dump(IConfiguration configuration, ILogger<Dump> logger, InstanceThing instanceThing)
        {
            _configuration = configuration;
            _logger = logger;
            _instanceThing = instanceThing;
        }

        public Task<int> OnExecuteAsync()
        {
            _logger.LogInformation(_instanceThing.From);
            foreach (var item in _configuration.AsEnumerable().Reverse())
            {
                _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
            }

            return Task.FromResult(1);
        }
    }

    public class Default : IDefaultCommand
    {
        private readonly IApplicationState _state;
        private readonly ILogger<Default> _logger;

        public Default(IApplicationState state, ILogger<Default> logger)
        {
            _state = state;
            _logger = logger;
        }

        public int Run(IApplicationState state)
        {
            Console.WriteLine("Hello World!");
            return 1;
        }
    }
}