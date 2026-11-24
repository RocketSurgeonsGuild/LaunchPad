using System.Net;
using System.Text.Json;

using Sample.Restful.Tests.Helpers;

namespace Sample.Restful.Tests;

// The operations validate their requests in the mediator; ZeroAlloc.AspNetCore's ZeroAllocProblemDetailsJoint turns a rejected request into a 422.
public class ValidationTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Answer_An_Invalid_Request_With_The_Failures_By_Property()
    {
        var response = await AlbaHost.Server.CreateClient().PostAsJsonAsync(
            "/rocket",
            new { serialNumber = "short", type = "FalconHeavy" },
            TestContext.CancellationToken
        );

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.UnprocessableEntity);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.CancellationToken));
        var failures = document.RootElement.GetProperty("errors").GetProperty("SerialNumber");
        await failures.GetArrayLength().Should().BeEqualTo(1);
        await failures[0].GetProperty("propertyName").GetString().Should().BeEqualTo("SerialNumber");
        await failures[0].GetProperty("errorMessage").GetString().Should().BeEqualTo("SerialNumber must be at least 10 characters.");
    }

    // The samples' strong IDs read the empty Guid (RejectEmpty = false): the operation's IsAssigned rule rejects it, before the handler looks it up.
    [Test]
    public async Task Should_Reject_The_Empty_Id()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync(
            new Uri($"/rocket/{Guid.Empty}", UriKind.Relative),
            TestContext.CancellationToken
        );

        await response.StatusCode.Should().Because(await response.Content.ReadAsStringAsync(TestContext.CancellationToken)).BeEqualTo(HttpStatusCode.UnprocessableEntity);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.CancellationToken));
        var failures = document.RootElement.GetProperty("errors").GetProperty("Id");
        await failures.GetArrayLength().Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Create_A_Valid_Request()
    {
        var response = await AlbaHost.Server.CreateClient().PostAsJsonAsync(
            "/rocket",
            new { serialNumber = "1234567890-valid", type = "FalconHeavy" },
            TestContext.CancellationToken
        );

        await response.StatusCode.Should().Because(await response.Content.ReadAsStringAsync(TestContext.CancellationToken)).BeEqualTo(HttpStatusCode.Created);
    }
}
