using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.LaunchRecords
{
    public class GetLaunchRecordTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public GetLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Get_A_LaunchRecord()
        {
            var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
               .Invoke(
                    async (context, clock) =>
                    {
                        var rocket = new ReadyRocket
                        {
                            Id = Guid.NewGuid(),
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
                mediator => mediator.Send(new GetLaunchRecord.Request { Id = record.Id })
            );

            response.Partner.Should().Be("partner");
            response.Payload.Should().Be("geo-fence-ftl");
            response.RocketType.Should().Be(RocketType.Falcon9);
            response.RocketSerialNumber.Should().Be("12345678901234");
            response.ScheduledLaunchDate.Should().Be(Instant.FromDateTimeOffset(record.ScheduledLaunchDate));
        }
    }
}