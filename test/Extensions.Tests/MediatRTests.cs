using FakeItEasy;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

namespace Extensions.Tests;


public class MediatRTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper))
{

    [Fact]
    public async Task Test1()
    {
        var builder = ConventionContextBuilder.Create(Imports.Instance, new Dictionary<object, object>(), []);
        var context = await ConventionContext.FromAsync(builder);
        var services = new ServiceCollection();
        new MediatRConvention().Register(context, new ConfigurationBuilder().Build(), services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services.Should().Contain(x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Transient);

        var r = services.BuildServiceProvider();
        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request());

        A
           .CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Test2()
    {
        var builder = ConventionContextBuilder.Create(Imports.Instance, new Dictionary<object, object>(), []);
        var context = await ConventionContext.FromAsync(builder);
        var services = new ServiceCollection();
        new MediatRConvention(
            new()
            {
                MediatorLifetime = ServiceLifetime.Singleton,
            }
        ).Register(context, new ConfigurationBuilder().Build(), services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services
           .Should()
           .Contain(
                x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Singleton
            );

        var r = services.BuildServiceProvider();

        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request());

        A
           .CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
           .MustHaveHappenedOnceExactly();
    }


    public class Request : IRequest
    {

    }

    private class TestHandler : IRequestHandler<Request>
    {
        public Task Handle(Request message, CancellationToken token) => Task.FromResult(Unit.Value);
    }
}
