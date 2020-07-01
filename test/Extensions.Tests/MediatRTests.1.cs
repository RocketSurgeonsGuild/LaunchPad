using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.MediatR;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;
#pragma warning disable CA1034

namespace Rocket.Surgery.Extensions.MediatR.Tests
{
    public class MediatRTests : AutoFakeTest
    {
        [Fact]
        public async Task Test1()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IAssemblyCandidateFinder>(new TestAssemblyCandidateFinder());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IConventionScanner>(
                new BasicConventionScanner(A.Fake<IServiceProviderDictionary>(), new MediatRConvention())
            );
            var builder = AutoFake.Resolve<ServicesBuilder>();
            builder.UseMediatR();

            var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

            builder.Services.AddSingleton(sub);

            builder.Services.Should().Contain(
                x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Transient
            );

            var r = builder.Build();

            var mediator = r.GetRequiredService<IMediator>();

            await mediator.Send(new Request()).ConfigureAwait(false);

            A.CallTo(() => sub.Handle(A<Request>._, A<CancellationToken>._, A<RequestHandlerDelegate<Unit>>._))
               .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Test2()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IAssemblyCandidateFinder>(new TestAssemblyCandidateFinder());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IConventionScanner>(
                new BasicConventionScanner(A.Fake<IServiceProviderDictionary>(), new MediatRConvention())
            );
            var builder = AutoFake.Resolve<ServicesBuilder>();
            builder.UseMediatR(new MediatRServiceConfiguration().AsSingleton());

            var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

            builder.Services.AddSingleton(sub);

            builder.Services.Should().Contain(
                x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Singleton
            );

            var r = builder.Build();

            var mediator = r.GetRequiredService<IMediator>();

            await mediator.Send(new Request()).ConfigureAwait(false);

            A.CallTo(() => sub.Handle(A<Request>._, A<CancellationToken>._, A<RequestHandlerDelegate<Unit>>._))
               .MustHaveHappenedOnceExactly();
        }

        public MediatRTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        private class TestAssemblyProvider : IAssemblyProvider
        {
            public IEnumerable<Assembly> GetAssemblies() => new[]
            {
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
                typeof(MediatRServicesExtensions).GetTypeInfo().Assembly
            };
        }

        private class TestAssemblyCandidateFinder : IAssemblyCandidateFinder
        {
            public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates) => new[]
            {
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
                typeof(MediatRServicesExtensions).GetTypeInfo().Assembly
            };
        }

        public class Request : IRequest { }

        private class TestHandler : IRequestHandler<Request>
        {
            public Task<Unit> Handle(Request message, CancellationToken token) => Task.FromResult(Unit.Value);
        }
    }
}