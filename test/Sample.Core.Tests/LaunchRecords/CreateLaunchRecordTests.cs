using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.LaunchRecords
{
    public class CreateLaunchRecordTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public CreateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Create_A_LaunchRecord()
        {
            var context = ServiceProvider.GetRequiredService<RocketDbContext>();
            var rocket = new ReadyRocket()
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            };
            context.Add(rocket);

            await context.SaveChangesAsync();

            var response = await ServiceProvider.WithScoped<IMediator, IClock>().Invoke(
                (mediator, clock) => mediator.Send(
                    new CreateLaunchRecord.Request()
                    {
                        Partner = "partner",
                        Payload = "geo-fence-ftl",
                        RocketId = rocket.Id,
                        ScheduledLaunchDate = clock.GetCurrentInstant(),
                        PayloadWeightKg = 100,
                    }
                )
            );

            response.Id.Should().NotBeEmpty();
        }
    }
}