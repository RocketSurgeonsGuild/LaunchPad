using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Rocket.Surgery.LaunchPad.Grpc.Validation;
using Sample.Core;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Grpc.Tests.Validation.Integration
{
    public class CustomValidatorIntegrationTest : LoggerTest, IClassFixture<TestWebHost>
    {
        private readonly ConventionTestWebHost<Startup> _factory;

        public CustomValidatorIntegrationTest(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
            => _factory = factory.ConfigureLoggerFactory(LoggerFactory);

        [Fact]
        public async Task Should_ResponseMessage_When_MessageIsValid()
        {
            await _factory.Services.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rockets = faker.Generate(3);
                        var records = new LaunchRecordFaker(rockets).Generate(10);
                        z.AddRange(rockets);
                        z.AddRange(records);
                        await z.SaveChangesAsync();
                    }
                );

            // Given
            var client = new Rockets.RocketsClient(_factory.CreateGrpcChannel());

            // When
            var response = await client.ListRocketsAsync(new ListRocketsRequest());

            // Then nothing happen.
            response.Results.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
        {
            // Given
            var client = new Rockets.RocketsClient(_factory.CreateGrpcChannel());

            // When
            async Task Action()
            {
                await client.GetRocketsAsync(new GetRocketRequest() { Id = "" });
            }

            // Then
            var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
            Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        }
    }
}