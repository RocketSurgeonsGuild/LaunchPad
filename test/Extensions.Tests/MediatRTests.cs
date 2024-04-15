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

public class MediatRTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Test1()
    {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var builder = new ConventionContextBuilder(new Dictionary<object, object>())
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = await ConventionContext.FromAsync(builder);
        var services = new ServiceCollection();
        new MediatRConvention().Register(context, new ConfigurationBuilder().Build(), services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services.Should().Contain(x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Transient);

        var r = services.BuildServiceProvider();
        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request());

        A.CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Test2()
    {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var builder = new ConventionContextBuilder(new Dictionary<object, object>())
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = await ConventionContext.FromAsync(builder);
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

        await mediator.Send(new Request());

        A.CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
         .MustHaveHappenedOnceExactly();
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

        public IEnumerable<Assembly> GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath = "", string memberName = "", int lineNumber = 0)
        {
            return new[]
            {
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
                typeof(MediatRConvention).GetTypeInfo().Assembly
            };
        }

        public IEnumerable<Type> GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath = "", string memberName = "", int lineNumber = 0)
        {
            return Enumerable.Empty<Type>();
        }
    }

    public class Request : IRequest;

    private class TestHandler : IRequestHandler<Request>
    {
        public Task Handle(Request message, CancellationToken token)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
