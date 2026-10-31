using FakeItEasy;

using Joinery.Dovetail;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace Extensions.Tests;

public class MediatRTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Test1()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder(), TestContext.CancellationToken);
        var services = new ServiceCollection();
        new MediatRJoint().Register(context, services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services.ShouldContain(x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Transient);

        var r = services.BuildServiceProvider();
        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request(), TestContext.CancellationToken);

        A
           .CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
           .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task Test2()
    {
        var builder = TestJoints
                     .CreateBuilder()
                     .Set(
                          new FoundationOptions
                          {
                              MediatorLifetime = ServiceLifetime.Singleton,
                          }
                      );
        var context = await DovetailContext.FromAsync(builder, TestContext.CancellationToken);
        var services = new ServiceCollection();
        new MediatRJoint().Register(context, services);

        var sub = A.Fake<IPipelineBehavior<Request, Unit>>();

        services.AddSingleton(sub);

        services
           .ShouldContain(
                x => x.ServiceType == typeof(IMediator) && x.Lifetime == ServiceLifetime.Singleton
            );

        var r = services.BuildServiceProvider();

        var mediator = r.GetRequiredService<IMediator>();

        await mediator.Send(new Request(), TestContext.CancellationToken);

        A
           .CallTo(() => sub.Handle(A<Request>._, A<RequestHandlerDelegate<Unit>>._, A<CancellationToken>._))
           .MustHaveHappenedOnceExactly();
    }

    public class Request : IRequest;

    private class TestHandler : IRequestHandler<Request>
    {
        public Task Handle(Request message, CancellationToken token) => Task.FromResult(Unit.Value);
    }
}
