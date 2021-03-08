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
            var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async (context, ct) =>
                    {
                        var rocket = new ReadyRocket()
                        {
                            Type = RocketType.Falcon9,
                            SerialNumber = "12345678901234"
                        };
                        context.Add(rocket);

                        await context.SaveChangesAsync(ct);
                        return rocket;
                    }
                );

            var response = await ServiceProvider.WithScoped<IMediator, IClock>().Invoke(
                async (mediator, clock, ct) => await mediator.Send(
                    new CreateLaunchRecord.Request()
                    {
                        Partner = "partner",
                        Payload = "geo-fence-ftl",
                        RocketId = rocket.Id,
                        ScheduledLaunchDate = clock.GetCurrentInstant(),
                        PayloadWeightKg = 100,
                    },
                    ct
                )
            );

            response.Id.Should().NotBeEmpty();
        }
    }
}