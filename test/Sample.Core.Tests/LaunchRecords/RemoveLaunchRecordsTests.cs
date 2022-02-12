using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
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

        await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new DeleteLaunchRecord.Request { Id = id })
        );

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
    }

    public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new();
}
