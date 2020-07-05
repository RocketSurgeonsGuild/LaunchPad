using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;
using Xunit;
using Xunit.Abstractions;
using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets
{
    public class CreateLaunchRecordTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public CreateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Create_A_LaunchRecord()
        {
            var rocket = await _serviceProvider.WithScoped<RocketDbContext>()
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

            var response = await _serviceProvider.WithScoped<IMediator, IClock>().Invoke(
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