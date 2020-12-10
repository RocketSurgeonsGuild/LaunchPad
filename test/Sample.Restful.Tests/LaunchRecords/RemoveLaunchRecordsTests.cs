using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Sample.Restful.Client;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests.LaunchRecords
{
    public class RemoveLaunchRecordsTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Remove_LaunchRecord()
        {
            var client = new LaunchRecordClient(Factory.CreateClient());
            var id = await ServiceProvider.WithScoped<RocketDbContext>()
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

            await client.RemoveLaunchRecordAsync(id);

            ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
        }
    }
}