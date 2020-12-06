using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Sample.BlazorServer
{
    [ImportConventions]
    public partial class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default), z => z.WithConventionsFrom(GetConventions))
           .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}