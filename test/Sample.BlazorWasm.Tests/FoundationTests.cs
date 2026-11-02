using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Sample.BlazorWasm.Tests;

public class FoundationTests : HandleTestHostBase
{
    [Test]
    public async Task Should_Handle_Requests_Through_The_Joints()
    {
        var response = await ServiceProvider
                            .GetRequiredService<IMediator>()
                            .Send(new TestHandler.Request { FirstName = "Ada", LastName = "Lovelace" }, TestContext.CancellationToken);

        response.FullName.ShouldBe("Ada Lovelace");
    }

    [Test]
    public async Task Should_Register_The_Validators_Of_The_Sample()
    {
        var result = await ServiceProvider
                          .GetRequiredService<IValidator<TestHandler.Request>>()
                          .ValidateAsync(new TestHandler.Request(), TestContext.CancellationToken);

        result.Errors.Select(z => z.PropertyName).Distinct().ShouldBe([nameof(TestHandler.Request.FirstName), nameof(TestHandler.Request.LastName)]);
    }
}
