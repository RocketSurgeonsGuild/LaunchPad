using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Sample.Restful.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.LaunchRecords
{
    public class CreateLaunchRecordTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public CreateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Create_A_LaunchRecord()
        {
            var client = new LaunchRecordClient(Factory.CreateClient());
            var clock = ServiceProvider.GetRequiredService<IClock>();
            var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var rocket = new ReadyRocket()
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
                new CreateLaunchRecordRequest()
                {
                    Partner = "partner",
                    Payload = "geo-fence-ftl",
                    RocketId = rocket.Id,
                    ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                    PayloadWeightKg = 100,
                }
            );

            response.Result.Id.Should().NotBeEmpty();
        }
    }
}