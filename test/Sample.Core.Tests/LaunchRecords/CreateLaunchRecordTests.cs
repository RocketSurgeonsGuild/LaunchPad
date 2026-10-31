using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;

using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class CreateLaunchRecordTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Create_A_LaunchRecord()
    {
        ReadyRocket rocket;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            rocket = new ReadyRocket
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234",
            };
            context.Add(rocket);

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();

        var response = await mediator.Send(
            new CreateLaunchRecord.Request
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id,
                ScheduledLaunchDate = clock.GetCurrentInstant(),
                PayloadWeightKg = 100,
            },
            TestContext.CancellationToken
        );

        response.Id.Value.ShouldNotBe(Guid.Empty);
    }

    private static readonly Faker Faker = new();
}
