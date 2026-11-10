using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Serilog.Events;
using ZeroAlloc.Mediator;
using ZeroAlloc.Mediator.Validation;

namespace Sample.Core.Tests.Rockets;

public class UpdateRocketTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Update_A_Rocket()
    {
        var rocket = await SeedRocketAsync();

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = ( await mediator.Send(
            new EditRocket.Request
            {
                Id = rocket.Id,
                Type = RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse()),
            },
            TestContext.CancellationToken
        ) ).Value.ShouldBeOfType<RocketModel>();

        response.Type.ShouldBe(RocketType.FalconHeavy);
        response.Sn.ShouldBe("43210987654321");
    }

    [Test]
    public async Task Should_Patch_A_Rocket()
    {
        var rocket = await SeedRocketAsync();

        var request = new EditRocket.PatchRequest
        {
            Id = rocket.Id,
            //            Type = RocketType.FalconHeavy,
            SerialNumber = string.Join("", rocket.SerialNumber.Reverse()),
        }.ResetChanges() with
        {
            Type = RocketType.FalconHeavy,
        };
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = ( await mediator.Send(request, TestContext.CancellationToken) ).Value.ShouldBeOfType<RocketModel>();

        response.Type.ShouldBe(RocketType.FalconHeavy);
        response.Sn.ShouldBe("12345678901234");
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocket.Request request, string propertyName)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> a = async () => await mediator.Send(request, TestContext.CancellationToken);
        ( await a.ShouldThrowAsync<ValidationFailedException>() ).Error.Failures.Select(x => x.PropertyName).ShouldContain(propertyName);
    }

    public static IEnumerable<Func<(EditRocket.Request Request, string PropertyName)>> RequiredFieldData()
    {
        yield return () => (
            new EditRocket.Request
            {
                Id = RocketId.New(),
            },
            nameof(EditRocket.Request.SerialNumber)
        );
        yield return () => (
            new EditRocket.Request
            {
                Id = RocketId.New(),
                SerialNumber = Faker.Random.String2(0, 9),
            },
            nameof(EditRocket.Request.SerialNumber)
        );
        yield return () => (
            new EditRocket.Request
            {
                Id = RocketId.New(),
                SerialNumber = Faker.Random.String2(600, 800),
            },
            nameof(EditRocket.Request.SerialNumber)
        );
    }

    private async Task<ReadyRocket> SeedRocketAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
        var rocket = new ReadyRocket
        {
            Type = RocketType.Falcon9,
            SerialNumber = "12345678901234",
        };
        context.Add(rocket);

        await context.SaveChangesAsync(TestContext.CancellationToken);
        return rocket;
    }

    private static readonly Faker Faker = new();
}
