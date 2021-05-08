using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets
{
    public class RemoveRocketsTests : HandleGrpcHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task Should_Remove_Rocket()
        {
            var client = new R.RocketsClient(Factory.CreateGrpcChannel());
            var id = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rocket = faker.Generate();
                        z.Add(rocket);

                        await z.SaveChangesAsync().ConfigureAwait(false);
                        return rocket.Id;
                    }
                );

            await client.DeleteRocketAsync(new DeleteRocketRequest { Id = id.ToString() });

            ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
        }
    }
}