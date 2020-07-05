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
    public class ListLaunchRecordsTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public ListLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_List_LaunchRecords()
        {
            await _serviceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rockets = faker.Generate(3);
                        var records = new LaunchRecordFaker(rockets).Generate(10);
                        z.AddRange(rockets);
                        z.AddRange(records);
                        await z.SaveChangesAsync();
                    }
                );

            var response = await _serviceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(new ListLaunchRecords.Request())
            );

            response.Should().HaveCount(10);
        }
    }
}