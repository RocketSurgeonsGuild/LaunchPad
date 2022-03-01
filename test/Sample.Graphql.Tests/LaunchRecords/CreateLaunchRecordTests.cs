﻿using Bogus;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Graphql.Tests.LaunchRecords;

public class CreateLaunchRecordTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );


        var response = await client.CreateLaunchRecord.ExecuteAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.Value,
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset().ToString("O"),
                PayloadWeightKg = 100,
            }
        );
        response.EnsureNoErrors();

        response.Data.CreateLaunchRecord.Id.Should().NotBeEmpty();
    }

    public CreateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
