using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using HttpRocketType = Sample.Restful.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets
{
    public class GetRocketTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task Should_Get_A_Rocket()
        {
            var client = new RocketClient(Factory.CreateClient());
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

            var response = await client.GetRocketAsync(rocket);

            response.Result.Type.Should().Be(HttpRocketType.Falcon9);
            response.Result.Sn.Should().Be("12345678901234");
        }
    }
}