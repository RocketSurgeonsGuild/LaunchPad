using System.Net;

using Microsoft.Extensions.DependencyInjection;

using Sample.BlazorServer.Tests.Helpers;
using Sample.Core.Operations.Rockets;

using ZeroAlloc.Validation;

namespace Sample.BlazorServer.Tests;

public class FoundationTests : WebAppFixtureTest
{
    [Test]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task Registers_The_Validators_Of_The_View_Models()
    {
        // the Create and Edit forms bind to the view models, and ZeroAllocValidator requires their validators
        var result = await AlbaHost.Services
                                   .GetRequiredService<ValidatorFor<CreateRocket.RequestViewModel>>()
                                   .ValidateAsync(new CreateRocket.RequestViewModel { SerialNumber = "" }, TestContext.CancellationToken);
        await result.Failures.ToArray().Select(z => z.PropertyName).Should().Contain(nameof(CreateRocket.RequestViewModel.SerialNumber));
        await AlbaHost.Services.GetService<ValidatorFor<EditRocket.RequestViewModel>>().Should().NotBeNull();
    }
}
