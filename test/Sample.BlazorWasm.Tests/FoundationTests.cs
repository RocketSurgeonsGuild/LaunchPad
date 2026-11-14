using Microsoft.Extensions.DependencyInjection;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.BlazorWasm.Tests;

public class FoundationTests : HandleTestHostBase
{
    [Test]
    public async Task Should_Handle_Requests_Through_The_Joints()
    {
        var response = await ServiceProvider
                            .GetRequiredService<IMediator>()
                            .Send(new TestHandler.Request { FirstName = "Ada", LastName = "Lovelace" }, TestContext.CancellationToken);

        await response.FullName.Should().BeEqualTo("Ada Lovelace");
    }

    [Test]
    public async Task Should_Register_The_Validators_Of_The_Sample()
    {
        var result = await ServiceProvider
                          .GetRequiredService<ValidatorFor<TestHandler.Request>>()
                          .ValidateAsync(new TestHandler.Request(), TestContext.CancellationToken);

        await result.Failures.ToArray().Select(z => z.PropertyName).Distinct().Should().BeEquivalentTo([nameof(TestHandler.Request.FirstName), nameof(TestHandler.Request.LastName)], CollectionOrdering.Matching);
    }
}
