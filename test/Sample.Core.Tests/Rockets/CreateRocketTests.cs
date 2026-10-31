using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;

using Serilog.Events;

using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets;

public class CreateRocketTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Create_A_Rocket()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(
            new CreateRocket.Request
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234",
            },
            TestContext.CancellationToken
        );

        response.Id.Value.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            context.Add(
                new ReadyRocket
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234",
                }
            );

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> action = () => mediator.Send(
            new CreateRocket.Request
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234",
            },
            TestContext.CancellationToken
        );
        (await action.ShouldThrowAsync<RequestFailedException>())
           .Title.ShouldBe("Rocket Creation Failed");
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(CreateRocket.Request request, string propertyName)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var a = () => mediator.Send(request, TestContext.CancellationToken);
        (await a.ShouldThrowAsync<ValidationException>()).Errors.Select(x => x.PropertyName).ShouldContain(propertyName);
    }

    public static IEnumerable<Func<(CreateRocket.Request Request, string PropertyName)>> RequiredFieldData()
    {
        yield return () => (new(), nameof(CreateRocket.Request.SerialNumber));
        yield return () => (
            new()
            {
                SerialNumber = Faker.Random.String2(0, 9),
            },
            nameof(CreateRocket.Request.SerialNumber)
        );
        yield return () => (
            new()
            {
                SerialNumber = Faker.Random.String2(600, 800),
            },
            nameof(CreateRocket.Request.SerialNumber)
        );
    }

    private static readonly Faker Faker = new();
}
