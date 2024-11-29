using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Restful.Client;
using HttpRocketType = Sample.Restful.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.LaunchRecords;

public class GetLaunchRecordTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
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

        var response = ( await client.GetLaunchRecordAsync(record.Id.Value) ).Result;

        response.Partner.Should().Be("partner");
        response.Payload.Should().Be("geo-fence-ftl");
        response.RocketType.Should().Be(HttpRocketType.Falcon9);
        response.RocketSerialNumber.Should().Be("12345678901234");
        response.ScheduledLaunchDate.Should().Be(record.ScheduledLaunchDate);
    }

    [Fact]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());

        Func<Task> action = () => client.GetLaunchRecordAsync(Guid.NewGuid());
        await action.Should().ThrowAsync<ApiException<ProblemDetails>>()
                    .Where(
                         z => z.StatusCode == 404 && z.Result.Status == 404 && z.Result.Title == "Not Found"
                     );
    }

    public GetLaunchRecordTests(ITestOutputHelper outputHelper, TestWebHost host) : base(outputHelper, host)
    {
    }

    private static readonly Faker Faker = new();
}
