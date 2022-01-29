using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Sample.Worker;

[ImportConventions]
public partial class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(GetConventions))
                   .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
    }
}
