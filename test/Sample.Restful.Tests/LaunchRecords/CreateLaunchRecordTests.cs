using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.LaunchRecords;

public class CreateLaunchRecordTests : HandleWebHostBase<Program>
{
    private static readonly Faker Faker = new();

    public CreateLaunchRecordTests(ITestOutputHelper outputHelper, TestWebHost<Program> host) : base(outputHelper, host)
    {
    }

    [Fact]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );


        var response = await client.CreateLaunchRecordAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.Value,
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                PayloadWeightKg = 100,
            }
        );

        response.Result.Id.Should().NotBeEmpty();
    }
}
