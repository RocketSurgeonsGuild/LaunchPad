using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using Xunit;
using Xunit.Abstractions;
using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets;

public class CreateRocketTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(
                new CreateRocket.Request
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234"
                }
            )
        );

        response.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      z.Add(
                                          new ReadyRocket
                                          {
                                              Type = RocketType.Falcon9,
                                              SerialNumber = "12345678901234"
                                          }
                                      );

                                      await z.SaveChangesAsync();
                                  }
                              );

        Func<Task> action = () => ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(
                new CreateRocket.Request
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234"
                }
            )
        );
        ( await action.Should().ThrowAsync<RequestFailedException>() )
           .And.Title.Should().Be("Rocket Creation Failed");
    }

    public CreateRocketTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new Faker();

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(CreateRocket.Request request, string propertyName)
    {
        using var scope = ServiceProvider.CreateScope();

        var mediater = scope.ServiceProvider.GetRequiredService<IMediator>();

        var a = () => mediater.Send(request);
        ( await a.Should().ThrowAsync<ValidationException>() ).And.Errors.Select(x => x.PropertyName).Should().Contain(propertyName);
    }

    private class ShouldValidateUsersRequiredFieldData : TheoryData<CreateRocket.Request, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(new CreateRocket.Request(), nameof(CreateRocket.Request.SerialNumber));
            Add(
                new CreateRocket.Request
                {
                    SerialNumber = Faker.Random.String2(0, 9)
                },
                nameof(CreateRocket.Request.SerialNumber)
            );
            Add(
                new CreateRocket.Request
                {
                    SerialNumber = Faker.Random.String2(600, 800)
                },
                nameof(CreateRocket.Request.SerialNumber)
            );
        }
    }
}
