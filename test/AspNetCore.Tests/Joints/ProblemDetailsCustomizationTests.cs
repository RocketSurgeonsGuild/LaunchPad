using System.Text.Json;

using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

/// <summary>
///     The <see cref="ProblemDetailsJoint" /> fills in the title and the type of problem details through <see cref="ProblemDetailsOptions.CustomizeProblemDetails" />.
/// </summary>
public class ProblemDetailsCustomizationTests
{
    private const string NotFoundTitle = "Not Found";
    private const string NotFoundLink = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
    private const int ClientClosedRequest = 499;

    [Test]
    public async Task Should_Fill_In_The_Title_And_Type_Of_A_Known_Client_Error()
    {
        using var provider = await CreateProvider();
        var context = CreateContext(provider, new() { Status = StatusCodes.Status404NotFound, });

        Customize(provider, context);

        await context.ProblemDetails.Title.Should().BeEqualTo(NotFoundTitle);
        await context.ProblemDetails.Type.Should().BeEqualTo(NotFoundLink);
    }

    [Test]
    public async Task Should_Keep_A_Title_And_Type_That_Are_Already_Set()
    {
        using var provider = await CreateProvider();
        var context = CreateContext(
            provider,
            new() { Status = StatusCodes.Status404NotFound, Title = "Rocket not found", Type = "https://example.com/not-found", }
        );

        Customize(provider, context);

        await context.ProblemDetails.Title.Should().BeEqualTo("Rocket not found");
        await context.ProblemDetails.Type.Should().BeEqualTo("https://example.com/not-found");
    }

    [Test]
    [Arguments(null)]
    [Arguments(StatusCodes.Status200OK)]
    public async Task Should_Leave_Problem_Details_Without_A_Known_Client_Error_Alone(int? status)
    {
        using var provider = await CreateProvider();
        var context = CreateContext(provider, new() { Status = status, });

        Customize(provider, context);

        await context.ProblemDetails.Title.Should().BeNull();
        await context.ProblemDetails.Type.Should().BeNull();
    }

    [Test]
    public async Task Should_Keep_The_Customization_That_Was_Already_Configured()
    {
        using var provider = await CreateProvider(
            services => services.AddProblemDetails(options => options.CustomizeProblemDetails = context => context.ProblemDetails.Extensions["traceId"] = "trace")
        );
        var context = CreateContext(provider, new() { Status = StatusCodes.Status404NotFound, });

        Customize(provider, context);

        await context.ProblemDetails.Extensions["traceId"].Should().BeEqualTo<object?>("trace");
        await context.ProblemDetails.Title.Should().BeEqualTo(NotFoundTitle);
    }

    [Test]
    public async Task Should_Write_The_Title_And_Type_Through_The_Problem_Details_Service()
    {
        // 499 is not one of the statuses the default writer knows, so only the customization can give it a title and a type
        using var provider = await CreateProvider(
            services => services.Configure<ApiBehaviorOptions>(
                options => options.ClientErrorMapping[ClientClosedRequest] = new() { Title = "Client Closed Request", Link = "https://example.com/499", }
            )
        );
        var httpContext = new DefaultHttpContext { RequestServices = provider, Response = { Body = new MemoryStream(), }, };
        httpContext.Response.StatusCode = ClientClosedRequest;

        var written = await provider
                           .GetRequiredService<IProblemDetailsService>()
                           .TryWriteAsync(new() { HttpContext = httpContext, ProblemDetails = new() { Status = ClientClosedRequest, }, });

        await written.Should().BeTrue();
        httpContext.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        await document.RootElement.GetProperty("title").GetString().Should().BeEqualTo("Client Closed Request");
        await document.RootElement.GetProperty("type").GetString().Should().BeEqualTo("https://example.com/499");
    }

    [Test]
    public async Task Should_Not_Fill_In_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<ProblemDetailsJoint>().CreateAsync();
        var services = new ServiceCollection();
        services.Configure<ApiBehaviorOptions>(ConfigureClientErrorMapping);
        await services.ApplyService(context);
        await services.ApplyService(context);
        using var provider = JointTestContext.BuildServiceProvider(services);
        var problemDetailsContext = CreateContext(provider, new() { Status = StatusCodes.Status404NotFound, });

        Customize(provider, problemDetailsContext);

        await problemDetailsContext.ProblemDetails.Title.Should().BeEqualTo(NotFoundTitle);
        await problemDetailsContext.ProblemDetails.Type.Should().BeEqualTo(NotFoundLink);
    }

    // ApiBehaviorOptions has no client error mapping until MVC configures it, so the tests that do not add MVC set the one they use
    private static void ConfigureClientErrorMapping(ApiBehaviorOptions options) =>
        options.ClientErrorMapping[StatusCodes.Status404NotFound] = new() { Title = NotFoundTitle, Link = NotFoundLink, };

    private static async Task<ServiceProvider> CreateProvider(Action<IServiceCollection>? configure = null)
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<ProblemDetailsJoint>().CreateAsync();
        var services = new ServiceCollection();
        services.Configure<ApiBehaviorOptions>(ConfigureClientErrorMapping);
        configure?.Invoke(services);
        await services.ApplyService(context);
        return JointTestContext.BuildServiceProvider(services);
    }

    private static ProblemDetailsContext CreateContext(IServiceProvider provider, ProblemDetails problemDetails) =>
        new() { HttpContext = new DefaultHttpContext { RequestServices = provider, }, ProblemDetails = problemDetails, };

    private static void Customize(IServiceProvider provider, ProblemDetailsContext context)
    {
        var customize = provider.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value.CustomizeProblemDetails ?? throw new InvalidOperationException("The problem details joint did not configure a customization");
        customize(context);
    }
}
