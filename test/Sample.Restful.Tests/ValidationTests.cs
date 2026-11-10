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

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.CancellationToken));
        var failures = document.RootElement.GetProperty("errors").GetProperty("SerialNumber");
        failures.GetArrayLength().ShouldBe(1);
        failures[0].GetProperty("propertyName").GetString().ShouldBe("SerialNumber");
        failures[0].GetProperty("errorMessage").GetString().ShouldBe("SerialNumber must be at least 10 characters.");
    }

    [Test]
    public async Task Should_Create_A_Valid_Request()
    {
        var response = await AlbaHost.Server.CreateClient().PostAsJsonAsync(
            "/rocket",
            new { serialNumber = "1234567890-valid", type = "FalconHeavy" },
            TestContext.CancellationToken
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created, await response.Content.ReadAsStringAsync(TestContext.CancellationToken));
    }
}
