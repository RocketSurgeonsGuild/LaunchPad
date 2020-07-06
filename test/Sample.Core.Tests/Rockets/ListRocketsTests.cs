using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentValidation;
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
    public class ListRocketsTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_List_Rockets()
        {
            await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        z.AddRange(faker.Generate(10));

                        await z.SaveChangesAsync();
                    }
                );

            var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(
                    new ListRockets.Request()
                        { }
                )
            );

            response.Should().HaveCount(10);
        }
    }
}