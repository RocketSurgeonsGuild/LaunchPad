using System.Reflection;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

namespace Extensions.Tests;

public class MediatRTests : AutoFakeTest
{
    [Fact]
    public async Task Test1()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = ConventionContext.From(builder);
        var services = new ServiceCollection();
        new MediatRConvention().Register(context, new ConfigurationBuilder().Build(), services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services.Should().Contain(x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Transient);

        var r = services.BuildServiceProvider();
        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request()).ConfigureAwait(false);

        A.CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Test2()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = ConventionContext.From(builder);
        var services = new ServiceCollection();
        new MediatRConvention(
            new FoundationOptions
            {
                MediatorLifetime = ServiceLifetime.Singleton
            }
        ).Register(context, new ConfigurationBuilder().Build(), services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services.Should().Contain(
            x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Singleton
        );

        var r = services.BuildServiceProvider();

        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request()).ConfigureAwait(false);

        A.CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
         .MustHaveHappenedOnceExactly();
    }

    public MediatRTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private class TestAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
                typeof(MediatRConvention).GetTypeInfo().Assembly
            };
        }
    }

    private class TestAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
        {
            return new[]
            {
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
                typeof(MediatRConvention).GetTypeInfo().Assembly
            };
        }
    }

    public class Request : IRequest
    {
    }

    private class TestHandler : IRequestHandler<Request>
    {
        public Task<Unit> Handle(Request message, CancellationToken token)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
