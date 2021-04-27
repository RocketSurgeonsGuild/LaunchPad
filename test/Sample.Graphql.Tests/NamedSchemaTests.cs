using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Conventions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Graphql.Tests
{
    public class NamedSchemaTests : LoggerTest, IAsyncLifetime
    {
        private readonly TestHostBuilder _hostBuilder;

        public NamedSchemaTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _hostBuilder = TestHost.For(this, LoggerFactory)
               .WithLogger(LoggerFactory.CreateLogger(nameof(TestHost)))
               .ExcludeConventions()
               .Create(z => z.AppendConvention<HotChocolateConvention>());
            _hostBuilder.ConfigureServices(
                (context, collection) =>
                {
                    collection.AddGraphQL();
                    collection.AddGraphQL("Named");
                }
            );
            ExcludeSourceContext(nameof(TestHost));
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}