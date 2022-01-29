using Bogus;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using Xunit;
using Xunit.Abstractions;
using Duration = NodaTime.Duration;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : HandleGrpcHostBase
{
    [Fact]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(Factory.CreateGrpcChannel());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clock) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Id = Guid.NewGuid(),
                                                       Type = Core.Domain.RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   context.Add(rocket);

                                                   var record = new LaunchRecord
                                                   {
                                                       Partner = "partner",
                                                       Payload = "geo-fence-ftl",
                                                       RocketId = rocket.Id,
                                                       Rocket = rocket,
                                                       ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                                                       PayloadWeightKg = 100,
                                                   };
                                                   context.Add(record);

                                                   await context.SaveChangesAsync();
                                                   return record;
                                               }
                                           );

        await client.EditLaunchRecordAsync(
            new UpdateLaunchRecordRequest
            {
                Id = record.Id.ToString(),
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.ToString(),
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset().ToTimestamp(),
                PayloadWeightKg = 200,
            }
        );

        var response = await client.GetLaunchRecordsAsync(new GetLaunchRecordRequest { Id = record.Id.ToString() });

        response.ScheduledLaunchDate.Should().Be(( record.ScheduledLaunchDate.ToInstant() + Duration.FromSeconds(1) ).ToDateTimeOffset().ToTimestamp());
        response.PayloadWeightKg.Should().Be(200);
    }

    public UpdateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new Faker();
}
