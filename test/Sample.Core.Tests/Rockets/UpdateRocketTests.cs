using Bogus;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using System;
using Xunit;
using Xunit.Abstractions;
using ValidationException = FluentValidation.ValidationException;

namespace Sample.Core.Tests.Rockets
{
    public class UpdateRocketTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public UpdateRocketTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Update_A_Rocket()
        {
            var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var rocket = new ReadyRocket()
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
                    new EditRocket.Request()
                    {
                        Id = rocket.Id,
                        Type = RocketType.FalconHeavy,
                        SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
                    }
                )
            );

            response.Type.Should().Be(RocketType.FalconHeavy);
            response.Sn.Should().Be("43210987654321");
        }

        [Theory]
        [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
        public void Should_Validate_Required_Fields(EditRocket.Request request, string propertyName)
        {
            using var scope = ServiceProvider.CreateScope();

            var mediater = scope.ServiceProvider.GetRequiredService<IMediator>();

            Func<Task> a = () => mediater.Send(request);
            a.Should().Throw<ValidationException>().And.Errors.Select(x => x.PropertyName).Should().Contain(propertyName);
        }

        class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocket.Request, string>
        {
            public ShouldValidateUsersRequiredFieldData()
            {
                Add(new EditRocket.Request()
                {
                    Id = Guid.NewGuid()
                }, nameof(EditRocket.Request.SerialNumber));
                Add(
                    new EditRocket.Request()
                    {
                        Id = Guid.NewGuid(),
                        SerialNumber = Faker.Random.String2(0, 9)
                    },
                    nameof(EditRocket.Request.SerialNumber)
                );
                Add(
                    new EditRocket.Request()
                    {
                        Id = Guid.NewGuid(),
                        SerialNumber = Faker.Random.String2(600, 800)
                    },
                    nameof(EditRocket.Request.SerialNumber)
                );
            }
        }
    }
}