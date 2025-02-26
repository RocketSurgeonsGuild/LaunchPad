using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;

using Serilog.Events;
using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets;

public class CreateRocketTests(ITestContextAccessor testContext) : HandleTestHostBase(testContext, LogEventLevel.Verbose)
{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var response = await ServiceProvider
                            .WithScoped<IMediator>()
                            .Invoke(
                                 mediator => mediator.Send(
                                     new CreateRocket.Request
                                     {
                                         Type = RocketType.Falcon9,
                                         SerialNumber = "12345678901234",
                                     }
                                 )
                             );

        response.Id.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        await ServiceProvider
             .WithScoped<RocketDbContext>()
             .Invoke(
                  async z =>
                  {
                      z.Add(
                          new ReadyRocket
                          {
                              Type = RocketType.Falcon9,
                              SerialNumber = "12345678901234",
                          }
                      );

                      await z.SaveChangesAsync();
                  }
              );

        Func<Task> action = () => ServiceProvider
                                 .WithScoped<IMediator>()
                                 .Invoke(
                                      mediator => mediator.Send(
                                          new CreateRocket.Request
                                          {
                                              Type = RocketType.Falcon9,
                                              SerialNumber = "12345678901234",
                                          }
                                      )
                                  );
        (await action.ShouldThrowAsync<RequestFailedException>())
           .Title.ShouldBe("Rocket Creation Failed");
    }

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(CreateRocket.Request request, string propertyName)
    {
        using var scope = ServiceProvider.CreateScope();

        var mediater = scope.ServiceProvider.GetRequiredService<IMediator>();

        var a = () => mediater.Send(request);
        (await a.ShouldThrowAsync<ValidationException>()).Errors.Select(x => x.PropertyName).ShouldContain(propertyName);
    }

    private static readonly Faker Faker = new();

    private sealed class ShouldValidateUsersRequiredFieldData : TheoryData<CreateRocket.Request, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(new(), nameof(CreateRocket.Request.SerialNumber));
            Add(
                new()
                {
                    SerialNumber = Faker.Random.String2(0, 9),
                },
                nameof(CreateRocket.Request.SerialNumber)
            );
            Add(
                new()
                {
                    SerialNumber = Faker.Random.String2(600, 800),
                },
                nameof(CreateRocket.Request.SerialNumber)
            );
        }
    }
}
