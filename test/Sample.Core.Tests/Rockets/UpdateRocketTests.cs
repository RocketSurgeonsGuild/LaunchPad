﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Serilog.Events;
using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets;

public class UpdateRocketTests(ITestContextAccessor testContext) : HandleTestHostBase(testContext, LogEventLevel.Verbose)
{
    [Fact]
    public async Task Should_Update_A_Rocket()
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
                                                   return rocket;
                                               }
                                           );

        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(
                new EditRocket.Request
                {
                    Id = rocket.Id,
                    Type = RocketType.FalconHeavy,
                    SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
                }
            )
        );

        response.Type.ShouldBe(RocketType.FalconHeavy);
        response.Sn.ShouldBe("43210987654321");
    }

    [Fact]
    public async Task Should_Patch_A_Rocket()
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
                                                   return rocket;
                                               }
                                           );

        var request = new EditRocket.PatchRequest
        {
            Id = rocket.Id,
            //            Type = RocketType.FalconHeavy,
            SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
        }.ResetChanges() with
        {
            Type = RocketType.FalconHeavy
        };
        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(request)
        );

        response.Type.ShouldBe(RocketType.FalconHeavy);
        response.Sn.ShouldBe("12345678901234");
    }

    private static readonly Faker Faker = new();

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocket.Request request, string propertyName)
    {
        using var scope = ServiceProvider.CreateScope();

        var mediater = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> a = () => mediater.Send(request);
        (await a.ShouldThrowAsync<ValidationException>()).Errors.Select(x => x.PropertyName).ShouldContain(propertyName);
    }

    private sealed class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocket.Request, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(
                new EditRocket.Request
                {
                    Id = RocketId.New()
                }, nameof(EditRocket.Request.SerialNumber)
            );
            Add(
                new EditRocket.Request
                {
                    Id = RocketId.New(),
                    SerialNumber = Faker.Random.String2(0, 9)
                },
                nameof(EditRocket.Request.SerialNumber)
            );
            Add(
                new EditRocket.Request
                {
                    Id = RocketId.New(),
                    SerialNumber = Faker.Random.String2(600, 800)
                },
                nameof(EditRocket.Request.SerialNumber)
            );
        }
    }
}
