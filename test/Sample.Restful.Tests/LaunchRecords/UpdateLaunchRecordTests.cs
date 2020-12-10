using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Sample.Restful.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.LaunchRecords
{
    public class UpdateLaunchRecordTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public UpdateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Update_A_LaunchRecord()
        {
            var client = new LaunchRecordClient(Factory.CreateClient());
            var clock = ServiceProvider.GetRequiredService<IClock>();
            var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
               .Invoke(
                    async (context, clock) =>
                    {
                        var rocket = new ReadyRocket()
                        {
                            Id = Guid.NewGuid(),
                            Type = RocketType.Falcon9,
                            SerialNumber = "12345678901234"
                        };

                        var record = new LaunchRecord()
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

            await client.UpdateLaunchRecordAsync(
                record.Id,
                new EditLaunchRecordModel()
                {
                    Partner = "partner",
                    Payload = "geo-fence-ftl",
                    RocketId = record.RocketId,
                    ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                    PayloadWeightKg = 200,
                }
            );

            var response = await client.GetLaunchRecordAsync(record.Id);

            response.Result.ScheduledLaunchDate.Should().Be(( record.ScheduledLaunchDate.ToInstant() + Duration.FromSeconds(1) ).ToDateTimeOffset());
            response.Result.PayloadWeightKg.Should().Be(200);
        }
    }
}