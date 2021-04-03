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

        [Fact]
        public async Task Should_Support_Unnamed_Schemas()
        {
            var query = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => query.OperationType).Returns(OperationType.Query);
            A.CallTo(() => query.SchemaName).Returns(null);
            A.CallTo(() => query.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Unnamed")
                       .Type(new StringType())
                       .Resolver(z => "Unnamed");
                }
            );

            using var host = _hostBuilder
               .Configure(
                    z => z.ConfigureServices((_, s) => s.AddSingleton(query))
                )
               .Build();

            var unnamedSchemaExecutor = await host.Services.WithScoped<IRequestExecutorResolver>().Invoke(z => z.GetRequestExecutorAsync());
            Logger.LogInformation(unnamedSchemaExecutor.Schema.Print());
            unnamedSchemaExecutor.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Unnamed");
            unnamedSchemaExecutor.Schema.QueryType.Fields.Should().NotContain(z => z.Name == "Named");
        }

        [Fact(Skip = "Might not be required anymore")]
        public async Task Should_Support_Named_And_Unnamed_Schemas()
        {
            var query = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => query.OperationType).Returns(OperationType.Query);
            A.CallTo(() => query.SchemaName).Returns(null);
            A.CallTo(() => query.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Unnamed")
                       .Type(new StringType())
                       .Resolver(z => "Unnamed");
                }
            );
            var namedQuery1 = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => namedQuery1.OperationType).Returns(OperationType.Query);
            A.CallTo(() => namedQuery1.SchemaName).Returns("Named1");
            A.CallTo(() => namedQuery1.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Named1")
                       .Type(new BooleanType())
                       .Resolver(z => "Named1");
                }
            );
            var namedQuery2 = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => namedQuery2.OperationType).Returns(OperationType.Query);
            A.CallTo(() => namedQuery2.SchemaName).Returns("Named2");
            A.CallTo(() => namedQuery2.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Named2")
                       .Type(new BooleanType())
                       .Resolver(z => "Named2");
                }
            );

            using var host = _hostBuilder
               .Configure(
                    z => z.ConfigureServices((_, s) => s.AddSingleton(query).AddSingleton(namedQuery1).AddSingleton(namedQuery2))
                )
               .Build();

            var namedSchemaExecutor1 = await host.Services.WithScoped<IRequestExecutorResolver>()
               .Invoke(z => z.GetRequestExecutorAsync("Named1"));
            Logger.LogInformation(namedSchemaExecutor1.Schema.Print());
            namedSchemaExecutor1.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Unnamed");
            namedSchemaExecutor1.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Named1");

            var namedSchemaExecutor2 = await host.Services.WithScoped<IRequestExecutorResolver>()
               .Invoke(z => z.GetRequestExecutorAsync("Named2"));
            Logger.LogInformation(namedSchemaExecutor2.Schema.Print());
            namedSchemaExecutor2.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Unnamed");
            namedSchemaExecutor2.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Named2");
        }

        [Fact]
        public async Task Should_Support_Mulitiple_Named()
        {
            var query = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => query.OperationType).Returns(OperationType.Query);
            A.CallTo(() => query.SchemaName).Returns(null);
            A.CallTo(() => query.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Unnamed")
                       .Type(new StringType())
                       .Resolver(z => "Unnamed");
                }
            );
            var namedQuery = A.Fake<IConfigureGraphqlRootType>();
            A.CallTo(() => namedQuery.OperationType).Returns(OperationType.Query);
            A.CallTo(() => namedQuery.SchemaName).Returns("Named");
            A.CallTo(() => namedQuery.Configure(A<IObjectTypeDescriptor>._)).Invokes(
                (IObjectTypeDescriptor descriptor) =>
                {
                    descriptor.Field("Named")
                       .Type(new BooleanType())
                       .Resolver(z => "Named");
                }
            );

            using var host = _hostBuilder
               .Configure(
                    z => z.ConfigureServices((_, s) => s.AddSingleton(query).AddSingleton(namedQuery))
                )
               .Build();

            var namedSchemaExecutor = await host.Services.WithScoped<IRequestExecutorResolver>()
               .Invoke(z => z.GetRequestExecutorAsync("Named"));
            Logger.LogInformation(namedSchemaExecutor.Schema.Print());
            namedSchemaExecutor.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Unnamed");
            namedSchemaExecutor.Schema.QueryType.Fields.Should().Contain(z => z.Name == "Named");
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}