using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using System.Diagnostics.CodeAnalysis;

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
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(GetConventions))
           .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}