using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Hosting;

namespace Sample.Classic.Restful;

/// <summary>
///     Startup interop (here for testing only or for 3.1 support)
/// </summary>
public static partial class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                   .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(Imports.GetConventions));
    }
}
