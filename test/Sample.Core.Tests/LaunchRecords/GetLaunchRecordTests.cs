using MediatR;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class GetLaunchRecordTests(ITestOutputHelper outputHelper) : HandleTestHostBase(outputHelper, LogEventLevel.Verbose)
{
    [Fact]
    public async Task Should_Get_A_LaunchRecord()
    {
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clock) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Id = RocketId.New(),
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };

                                                   var record = new LaunchRecord
                                                   {
                                                       Partner = "partner",
                                                       Payload = "geo-fence-ftl",
                                                       RocketId = rocket.Id,
                                                       ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                                                       PayloadWeightKg = 100,
                                                   };
                                                   context.Add(rocket);
                                                   context.Add(record);

                                                   await context.SaveChangesAsync();
                                                   return record;
                                               }
                                           );

        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new GetLaunchRecord.Request( record.Id))
        );

        response.Partner.Should().Be("partner");
        response.Payload.Should().Be("geo-fence-ftl");
        response.RocketType.Should().Be(RocketType.Falcon9);
        response.RocketSerialNumber.Should().Be("12345678901234");
        response.ScheduledLaunchDate.Should().Be(Instant.FromDateTimeOffset(record.ScheduledLaunchDate));
    }

    [Fact]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        Func<Task> action = () => ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new GetLaunchRecord.Request (LaunchRecordId.New()))
        );

        await action.Should().ThrowAsync<NotFoundException>();
    }

    private static readonly Faker Faker = new();
}
