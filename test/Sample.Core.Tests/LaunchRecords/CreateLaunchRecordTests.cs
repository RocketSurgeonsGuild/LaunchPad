using MediatR;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class CreateLaunchRecordTests(ITestContextAccessor testContext) : HandleTestHostBase(testContext, LogEventLevel.Verbose)
{
    [Fact]
    public async Task Should_Create_A_LaunchRecord()
    {
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async (context, ct) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   context.Add(rocket);

                                                   await context.SaveChangesAsync(ct);
                                                   return rocket;
                                               },
                                               TestContext.CancellationToken
                                           );

        var response = await ServiceProvider.WithScoped<IMediator, IClock>().Invoke(
            (mediator, clock, ct) => mediator.Send(
                new CreateLaunchRecord.Request
                {
                    Partner = "partner",
                    Payload = "geo-fence-ftl",
                    RocketId = rocket.Id,
                    ScheduledLaunchDate = clock.GetCurrentInstant(),
                    PayloadWeightKg = 100,
                },
                ct
            ),
            TestContext.CancellationToken
        );

        response.Id.Value.ShouldNotBe(Guid.Empty);
    }

    private static readonly Faker Faker = new();
}
