using Microsoft.Extensions.DependencyModel;

ing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Sample.Grpc
{
    [ImportConventions]
    public partial class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit ht
        {
            return Host.CreateDefaultBuilder(args)
                       .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(GetConventions))
                       .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
