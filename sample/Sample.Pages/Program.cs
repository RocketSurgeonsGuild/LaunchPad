using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Sample.Pages;

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
                   .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                   .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(GetConventions));
    }
}
