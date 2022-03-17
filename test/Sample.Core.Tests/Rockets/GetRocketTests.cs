﻿using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.Rockets;

public class GetRocketTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_Get_A_Rocket()
    {
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
                                                   return rocket.Id;
                                               }
                                           );

        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new GetRocket.Request { Id = rocket })
        );

        response.Type.Should().Be(RocketType.Falcon9);
        response.Sn.Should().Be("12345678901234");
    }

    [Fact]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        Func<Task> action = () => ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new GetRocket.Request { Id = RocketId.New() })
        );

        await action.Should().ThrowAsync<NotFoundException>();
    }

    public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new();
}
