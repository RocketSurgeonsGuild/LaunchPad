extern alias httpclient;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Generators.Tests;

/// <summary>A server that also calls other services references several transport packages; their generated helpers must not collide.</summary>
public class CombinedGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Compile_A_Server_And_A_Client_In_One_Project()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Server(Mediator.MediatR, contracts)
                          .WithGenerator<httpclient::Rocket.Surgery.LaunchPad.Api.Generators.HttpClientGenerator>()
                          .AddReferences(typeof(IHttpClientFactory).Assembly, typeof(HttpClientFactoryServiceCollectionExtensions).Assembly, typeof(JsonContent).Assembly)
                          .AddSources(Contracts.ServerHost, Contracts.ClientHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
    }
}
