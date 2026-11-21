using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

using Rocket.Surgery.LaunchPad.Testing.Authorization;

using Sample.Minimal.Tests.Helpers;

namespace Sample.Minimal.Tests.Authorization;

/// <summary>
///     The policies of the operations, over HTTP: a caller without a token is challenged, and a caller whose token lacks the scope of the
///     operation is forbidden, each as the problem details of its union case.
/// </summary>
public class AuthorizationTests : WebAppFixtureTest
{
    [Test]
    public async Task A_Caller_Without_A_Token_Is_Challenged_For_The_Scope()
    {
        using var http = AlbaHost.Server.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/rockets", UriKind.Relative));
        request.Headers.Add(TestTokens.AnonymousHeader, "true");

        using var response = await http.SendAsync(request, TestContext.CancellationToken);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer scope=\"rockets:read\"");
        var problem = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.CancellationToken))!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Unauthorized");
    }

    [Test]
    public async Task A_Caller_Whose_Token_Lacks_The_Scope_Is_Forbidden()
    {
        using var http = AlbaHost.Server.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Delete, new Uri($"/rockets/{Guid.NewGuid()}", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Create("reader", "rockets:read"));

        using var response = await http.SendAsync(request, TestContext.CancellationToken);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer error=\"insufficient_scope\", scope=\"rockets:write\"");
        var problem = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.CancellationToken))!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Forbidden");
        await problem["missingScopes"]!.GetValue<string>().Should().BeEqualTo("rockets:write");
    }

    [Test]
    public async Task A_Caller_With_The_Scope_Is_Answered()
    {
        using var http = AlbaHost.Server.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/rockets", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Create("reader", "rockets:read"));

        using var response = await http.SendAsync(request, TestContext.CancellationToken);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task A_Token_The_Application_Does_Not_Trust_Is_Not_A_Caller()
    {
        using var http = AlbaHost.Server.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/rockets", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJub25lIn0.eyJzdWIiOiJtYWxsb3J5Iiwic2NvcGUiOiJyb2NrZXRzOnJlYWQifQ.");

        using var response = await http.SendAsync(request, TestContext.CancellationToken);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
    }
}
