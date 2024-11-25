using System.Runtime.CompilerServices;
using DiffEngine;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

namespace AspNetCore.FluentValidation.OpenApi.Tests;

class StringContainer
{
    public required string Value { get; set; }
}
class NullableStringContainer
{
    public string? Value { get; set; }
}
class CollectionContainer
{
    public List<string> Value { get; set; } = [];
}
class NullableIntegerContainer
{
    public int? Value { get; set; }
}
class IntegerContainer
{
    public int Value { get; set; }
}
class BooleanContainer
{
    public bool Value { get; set; }
}
class NullableBooleanContainer
{
    public bool? Value { get; set; }
}
class DecimalContainer
{
    public decimal Value { get; set; }
}
class NullableDecimalContainer
{
    public decimal? Value { get; set; }
}

[Experimental(Constants.ExperimentalId)]
public class RequiredPropertyRuleTests : RuleTestBase
{
    [Theory]
    [MemberData(nameof(GetTheoryData))]
    public  Task Should_generate_required_property(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    public static IEnumerable<object[]> GetTheoryData()
    {
        yield return [GetOpenApiDocument<BooleanContainer>("/default/boolean", x => x.RuleFor(y => y.Value))];
        yield return [GetOpenApiDocument<CollectionContainer>("/default/collection", x => x.RuleFor(y => y.Value))];
        yield return [GetOpenApiDocument<DecimalContainer>("/default/decimal", x => x.RuleFor(y => y.Value))];
        yield return [GetOpenApiDocument<IntegerContainer>("/default/integer", x => x.RuleFor(y => y.Value))];
        yield return [GetOpenApiDocument<StringContainer>("/default/string", x => x.RuleFor(z => z.Value))];

        yield return [GetOpenApiDocument<BooleanContainer>("/notempty/boolean", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<CollectionContainer>("/notempty/collection", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<DecimalContainer>("/notempty/decimal", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<IntegerContainer>("/notempty/integer", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<StringContainer>("/notempty/string", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableBooleanContainer>("/notempty/nullable/boolean", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/notempty/nullable/decimal", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/notempty/nullable/integer", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableStringContainer>("/notempty/nullable/string", x => x.RuleFor(y => y.Value).NotEmpty())];

        yield return [GetOpenApiDocument<BooleanContainer>("/notnull/boolean", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<CollectionContainer>("/notnull/collection", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<DecimalContainer>("/notnull/decimal", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<IntegerContainer>("/notnull/integer", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<StringContainer>("/notnull/string", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<NullableBooleanContainer>("/notnull/nullable/boolean", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/notnull/nullable/decimal", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/notnull/nullable/integer", x => x.RuleFor(y => y.Value).NotNull())];
        yield return [GetOpenApiDocument<NullableStringContainer>("/notnull/nullable/string", x => x.RuleFor(y => y.Value).NotNull())];

    }
}

public record OpenApiResult(string Path, Func<Task<string>> JsonFactory)
{
    public override string ToString() => Path;
}

static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DiffRunner.Disabled = true;
        DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode, DiffTool.VisualStudio);
//        Verify
    }
}

#pragma warning disable RSGEXP
public abstract class RuleTestBase
    #pragma warning restore RSGEXP
{
    protected static OpenApiResult GetOpenApiDocument<TObject>(string path, Action<InlineValidator<TObject>> configureValidator)
    {
        return new(
            path,
            async () =>
            {
                var builder = WebApplication.CreateSlimBuilder();
                builder.WebHost.UseTestServer();
                builder.Services.AddOpenApi();
                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddFluentValidationOpenApi();
                var validator = new InlineValidator<TObject>();
                configureValidator(validator);
                builder.Services.AddSingleton<IValidator<TObject>>(validator);

                await using var app = builder.Build();

                app.MapOpenApi();

                app.MapPost(path, (TObject container) => container);

                await app.StartAsync().ConfigureAwait(false);
                var testServer = (TestServer)app.Services.GetRequiredService<IServer>();
                var _client = testServer.CreateClient();

                var response = await _client.GetAsync("/openapi/v1.json");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        );
    }
}
