﻿using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords
{
    public class RemoveLaunchRecordsTests : HandleGrpcHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task Should_Remove_LaunchRecord()
        {
            var client = new LR.LaunchRecordsClient(Factory.CreateGrpcChannel());
            var id = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rocket = faker.Generate();
                        var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
                        z.Add(rocket);
                        z.Add(record);

                        await z.SaveChangesAsync();
                        return record.Id;
                    }
                );

            await client.DeleteLaunchRecordAsync(new DeleteLaunchRecordRequest { Id = id.ToString() });

            ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
        }
    }
}