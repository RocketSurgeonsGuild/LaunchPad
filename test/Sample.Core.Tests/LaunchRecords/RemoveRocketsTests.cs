using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.Rockets
{
    public class RemoveLaunchRecordsTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Remove_LaunchRecord()
        {
            var id = await _serviceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rocket = faker.Generate();
                        var record = new LaunchRecordFaker(new [] { rocket }.ToList()).Generate();
                        z.Add(rocket);
                        z.Add(record);

                        await z.SaveChangesAsync();
                        return record.Id;
                    }
                );

            await _serviceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(new DeleteLaunchRecord.Request() { Id = id})
            );

            _serviceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
        }
    }
}