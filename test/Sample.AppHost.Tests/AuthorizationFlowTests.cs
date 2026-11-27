using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;

namespace Sample.AppHost.Tests;

/// <summary>
///     The samples' authorization as a caller meets it under Sample.AppHost: tokens come from the Keycloak realm, the API samples validate
///     them, and the policies of the operations require their scopes.
/// </summary>
[RequiresDocker]
[ClassDataSource<AppHostFixture>(Shared = SharedType.PerTestSession)]
public partial class AuthorizationFlowTests(AppHostFixture host)
{
    [Test]
    public async Task A_Caller_Without_A_Token_Is_Challenged_For_The_Scope()
    {
        using var response = await host.Send("minimal", HttpMethod.Get, "/rockets");

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer scope=\"rockets:read\"");
        var problem = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Unauthorized");
    }

    [Test]
    public async Task A_Service_Reads_With_Its_Default_Scopes_And_Is_Forbidden_To_Write()
    {
        var token = await host.ServiceToken();

        using var read = await host.Send("minimal", HttpMethod.Get, "/rockets", token);
        using var write = await host.Send("minimal", HttpMethod.Delete, $"/rockets/{Guid.NewGuid()}", token);

        await read.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await write.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
        await write.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer error=\"insufficient_scope\", scope=\"rockets:write\"");
        var problem = JsonNode.Parse(await write.Content.ReadAsStringAsync())!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Forbidden");
        await problem["missingScopes"]!.GetValue<string>().Should().BeEqualTo("rockets:write");
    }

    [Test]
    public async Task A_Service_That_Asks_For_The_Write_Scope_Reaches_The_Handler()
    {
        var token = await host.ServiceToken("rockets:write");

        using var response = await host.Send("minimal", HttpMethod.Delete, $"/rockets/{Guid.NewGuid()}", token);

        // The policy passed: the handler ran and found no such rocket.
        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task The_Document_Declares_The_Realm_As_The_Scheme_Of_Its_Operations()
    {
        using var response = await host.Send("minimal", HttpMethod.Get, "/openapi/v1.json");
        var document = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        var flow = document["components"]!["securitySchemes"]!["oauth2"]!["flows"]!["authorizationCode"]!;
        await flow["authorizationUrl"]!.GetValue<string>().Should().BeEqualTo($"{host.Realm}/protocol/openid-connect/auth");
        await flow["tokenUrl"]!.GetValue<string>().Should().BeEqualTo($"{host.Realm}/protocol/openid-connect/token");
        await flow["scopes"]!.AsObject().Select(s => s.Key).Should().BeEquivalentTo(["launch-records:read", "launch-records:write", "rockets:read", "rockets:write"]);
        await document["paths"]!["/rockets"]!["get"]!["security"]!.ToJsonString().Should().BeEqualTo("""[{"oauth2":["rockets:read"]}]""");
    }

    /// <summary>
    ///     What an API explorer does when a caller presses Authorize: the authorization code flow with PKCE as the public client, signed in
    ///     as a user, with the token exchanged from the browser (so Keycloak must allow the explorer's origin).
    /// </summary>
    [Test]
    public async Task A_User_Signs_In_From_The_Api_Explorer_And_Calls_With_The_Scopes_Granted()
    {
        var explorer = host.Endpoint("minimal");
        var redirect = new Uri(explorer, "/swagger/oauth2-redirect.html").ToString();
        var verifier = Base64Url(RandomNumberGenerator.GetBytes(40));
        var challenge = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
        using var browser = new HttpClient(
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                AllowAutoRedirect = false,
                CookieContainer = new(),
            }
        );

        // the sign-in page
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["response_type"] = "code";
        query["client_id"] = "sample-web";
        query["redirect_uri"] = redirect;
        query["scope"] = "rockets:read rockets:write";
        query["state"] = "state";
        query["code_challenge"] = challenge;
        query["code_challenge_method"] = "S256";
        var page = await browser.GetStringAsync(new Uri($"{host.Realm}/protocol/openid-connect/auth?{query}"));
        var action = WebUtility.HtmlDecode(FormAction().Match(page).Groups[1].Value);

        // alice signs in, and Keycloak sends the browser back to the explorer with a code
        using var signedIn = await browser.PostAsync(
            new Uri(action),
            new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal) { ["username"] = "alice", ["password"] = "alice" })
        );
        await signedIn.StatusCode.Should().BeEqualTo(HttpStatusCode.Found);
        var location = signedIn.Headers.Location!;
        await location.GetLeftPart(UriPartial.Path).Should().BeEqualTo(redirect);
        var code = HttpUtility.ParseQueryString(location.Query)["code"]!;

        // the explorer's page exchanges the code, from its own origin
        using var exchange = new HttpRequestMessage(HttpMethod.Post, new Uri($"{host.Realm}/protocol/openid-connect/token"))
        {
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["grant_type"] = "authorization_code",
                    ["client_id"] = "sample-web",
                    ["code"] = code,
                    ["redirect_uri"] = redirect,
                    ["code_verifier"] = verifier,
                }
            ),
        };
        var origin = explorer.GetLeftPart(UriPartial.Authority);
        exchange.Headers.Add("Origin", origin);
        using var tokens = await browser.SendAsync(exchange);
        await tokens.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await tokens.Headers.GetValues("Access-Control-Allow-Origin").Single().Should().BeEqualTo(origin);
        var token = JsonNode.Parse(await tokens.Content.ReadAsStringAsync())!["access_token"]!.GetValue<string>();

        using var granted = await host.Send("minimal", HttpMethod.Get, "/rockets", token);
        using var notGranted = await host.Send("minimal", HttpMethod.Get, "/launch-records", token);

        await granted.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await notGranted.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task A_GraphQL_Mutation_Is_Rejected_Without_A_Token_And_Runs_With_Its_Scope()
    {
        const string mutation = """{"query":"mutation { deleteRocket(request: { id: \"00000000-0000-0000-0000-000000000001\" }) }"}""";
        var token = await host.ServiceToken("rockets:write");

        using var anonymous = await host.Send("graphql", HttpMethod.Post, "/graphql", content: new StringContent(mutation, Encoding.UTF8, "application/json"));
        using var allowed = await host.Send("graphql", HttpMethod.Post, "/graphql", token, new StringContent(mutation, Encoding.UTF8, "application/json"));

        var rejected = JsonNode.Parse(await anonymous.Content.ReadAsStringAsync())!["errors"]![0]!["extensions"]!;
        await rejected["code"]!.GetValue<string>().Should().BeEqualTo("UNAUTHENTICATED");
        await rejected["case"]!.GetValue<string>().Should().BeEqualTo("Unauthorized");
        // The policy passed: the handler ran and found no such rocket.
        var ran = JsonNode.Parse(await allowed.Content.ReadAsStringAsync())!["errors"]![0]!["extensions"]!;
        await ran["case"]!.GetValue<string>().Should().BeEqualTo("NotFound");
    }

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    [GeneratedRegex("<form[^>]*action=\"([^\"]+)\"")]
    private static partial Regex FormAction();
}
