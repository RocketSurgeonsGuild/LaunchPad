using System.Text.RegularExpressions;

using Argon;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

namespace AspNetCore.FluentValidation.OpenApi.Tests;

[Experimental(Constants.ExperimentalId)]
public partial class PropertyRuleHandlerTests : RuleTestBase
{
    [Theory]
    [MemberData(nameof(GetNotNullValidators))]
    public Task Should_generate_required_property(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(GetNotEmptyValidators))]
    public Task Should_generate_required_property_and_min_length(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(GetLengthValidators))]
    public Task Should_generate_length_properties(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(RegularExpressionValidators))]
    public Task Should_generate_regex_properties(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(EmailAddressValidators))]
    public Task Should_generate_email_properties(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(ComparisonValidators))]
    public Task Should_generate_comparison_properties(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    [Theory]
    [MemberData(nameof(BetweenValidators))]
    public Task Should_generate_between_properties(OpenApiResult result) => VerifyJson(result.JsonFactory()).UseParameters(result.Path);

    public static IEnumerable<object[]> GetNotNullValidators()
    {
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

    public static IEnumerable<object[]> GetNotEmptyValidators()
    {
        yield return [GetOpenApiDocument<BooleanContainer>("/notempty/boolean", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<CollectionContainer>("/notempty/collection", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<DecimalContainer>("/notempty/decimal", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<IntegerContainer>("/notempty/integer", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<StringContainer>("/notempty/string", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableBooleanContainer>("/notempty/nullable/boolean", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/notempty/nullable/decimal", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/notempty/nullable/integer", x => x.RuleFor(y => y.Value).NotEmpty())];
        yield return [GetOpenApiDocument<NullableStringContainer>("/notempty/nullable/string", x => x.RuleFor(y => y.Value).NotEmpty())];
    }

    public static IEnumerable<object[]> GetLengthValidators()
    {
        yield return [GetOpenApiDocument<StringContainer>("/min-length-5/string", x => x.RuleFor(y => y.Value).MinimumLength(5))];
        yield return [GetOpenApiDocument<StringContainer>("/max-length-100/string", x => x.RuleFor(y => y.Value).MaximumLength(100))];
        yield return [GetOpenApiDocument<StringContainer>("/length-5-50/string", x => x.RuleFor(y => y.Value).Length(5, 50))];
        yield return [GetOpenApiDocument<NullableStringContainer>("/min-length-5/nullable/string", x => x.RuleFor(y => y.Value).MinimumLength(5))];
        yield return [GetOpenApiDocument<NullableStringContainer>("/max-length-100/nullable/string", x => x.RuleFor(y => y.Value).MaximumLength(100))];
        yield return [GetOpenApiDocument<NullableStringContainer>("/length-5-50/nullable/string", x => x.RuleFor(y => y.Value).Length(5, 50))];
        yield return [GetOpenApiDocument<CollectionContainer>("/min-length-5/collection", x => x.RuleForEach(y => y.Value).MinimumLength(5))];
        yield return [GetOpenApiDocument<CollectionContainer>("/max-length-100/collection", x => x.RuleForEach(y => y.Value).MaximumLength(100))];
        yield return [GetOpenApiDocument<CollectionContainer>("/length-5-50/collection", x => x.RuleForEach(y => y.Value).Length(5, 50))];
    }

    public static IEnumerable<object[]> RegularExpressionValidators()
    {
        yield return [GetOpenApiDocument<StringContainer>("/regex/string", x => x.RuleFor(y => y.Value).Matches(MyRegex()))];
        yield return [GetOpenApiDocument<NullableStringContainer>("/regex/nullable/string", x => x.RuleFor(y => y.Value).Matches(MyRegex()))];
        yield return [GetOpenApiDocument<CollectionContainer>("/regex/collection", x => x.RuleForEach(y => y.Value).Matches(MyRegex1()))];
    }

    public static IEnumerable<object[]> EmailAddressValidators()
    {
        yield return [GetOpenApiDocument<StringContainer>("/email/string", x => x.RuleFor(y => y.Value).EmailAddress())];
        yield return [GetOpenApiDocument<NullableStringContainer>("/email/nullable/string", x => x.RuleFor(y => y.Value).EmailAddress())];
        yield return [GetOpenApiDocument<CollectionContainer>("/email/collection", x => x.RuleForEach(y => y.Value).EmailAddress())];
    }

    public static IEnumerable<object[]> ComparisonValidators()
    {
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/gt", x => x.RuleFor(y => y.Value).GreaterThan(2.1m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/gte", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2.1m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/lt", x => x.RuleFor(y => y.Value).LessThan(1.1m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/lte", x => x.RuleFor(y => y.Value).LessThanOrEqualTo(1.1m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/gtlte", x => x.RuleFor(y => y.Value).GreaterThan(1.1m).LessThanOrEqualTo(2.2m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/comparison/decimal/gtelt", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(1.1m).LessThan(2.2m))];

        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/gt", x => x.RuleFor(y => y.Value).GreaterThan(2))];
        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/gte", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2))];
        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/lt", x => x.RuleFor(y => y.Value).LessThan(1))];
        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/lte", x => x.RuleFor(y => y.Value).LessThanOrEqualTo(1))];
        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/gtlte", x => x.RuleFor(y => y.Value).GreaterThan(2).LessThanOrEqualTo(10))];
        yield return [GetOpenApiDocument<IntegerContainer>("/comparison/integer/gtelt", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2).LessThan(11))];

        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/gt", x => x.RuleFor(y => y.Value).GreaterThan(2))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/gte", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/lt", x => x.RuleFor(y => y.Value).LessThan(1))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/lte", x => x.RuleFor(y => y.Value).LessThanOrEqualTo(1))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/gtlte", x => x.RuleFor(y => y.Value).GreaterThan(2).LessThanOrEqualTo(10))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/comparison/nullable/integer/gtelt", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2).LessThan(11))];

        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/gt", x => x.RuleFor(y => y.Value).GreaterThan(2.1m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/gte", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(2.1m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/lt", x => x.RuleFor(y => y.Value).LessThan(1.1m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/lte", x => x.RuleFor(y => y.Value).LessThanOrEqualTo(1.1m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/gtlte", x => x.RuleFor(y => y.Value).GreaterThan(1.1m).LessThanOrEqualTo(2.2m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/comparison/nullable/decimal/gtelt", x => x.RuleFor(y => y.Value).GreaterThanOrEqualTo(1.1m).LessThan(2.2m))];
    }

    public static IEnumerable<object[]> BetweenValidators()
    {
        yield return [GetOpenApiDocument<DecimalContainer>("/between/exclusive/decimal/gtlte", x => x.RuleFor(y => y.Value).ExclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/between/exclusive/decimal/gtelt", x => x.RuleFor(y => y.Value).ExclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<IntegerContainer>("/between/exclusive/integer/gtlte", x => x.RuleFor(y => y.Value).ExclusiveBetween(2, 10))];
        yield return [GetOpenApiDocument<IntegerContainer>("/between/exclusive/integer/gtelt", x => x.RuleFor(y => y.Value).ExclusiveBetween(2, 11))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/between/exclusive/nullable/integer/gtlte", x => x.RuleFor(y => y.Value).ExclusiveBetween(2, 10))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/between/exclusive/nullable/integer/gtelt", x => x.RuleFor(y => y.Value).ExclusiveBetween(2, 11))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/between/exclusive/nullable/decimal/gtlte", x => x.RuleFor(y => y.Value).ExclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/between/exclusive/nullable/decimal/gtelt", x => x.RuleFor(y => y.Value).ExclusiveBetween(1.1m, 2.2m))];

        yield return [GetOpenApiDocument<DecimalContainer>("/between/inclusive/decimal/gtlte", x => x.RuleFor(y => y.Value).InclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<DecimalContainer>("/between/inclusive/decimal/gtelt", x => x.RuleFor(y => y.Value).InclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<IntegerContainer>("/between/inclusive/integer/gtlte", x => x.RuleFor(y => y.Value).InclusiveBetween(2, 10))];
        yield return [GetOpenApiDocument<IntegerContainer>("/between/inclusive/integer/gtelt", x => x.RuleFor(y => y.Value).InclusiveBetween(2, 11))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/between/inclusive/nullable/integer/gtlte", x => x.RuleFor(y => y.Value).InclusiveBetween(2, 10))];
        yield return [GetOpenApiDocument<NullableIntegerContainer>("/between/inclusive/nullable/integer/gtelt", x => x.RuleFor(y => y.Value).InclusiveBetween(2, 11))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/between/inclusive/nullable/decimal/gtlte", x => x.RuleFor(y => y.Value).InclusiveBetween(1.1m, 2.2m))];
        yield return [GetOpenApiDocument<NullableDecimalContainer>("/between/inclusive/nullable/decimal/gtelt", x => x.RuleFor(y => y.Value).InclusiveBetween(1.1m, 2.2m))];
    }

    private static OpenApiResult GetOpenApiDocument<TObject>(string path, Action<InlineValidator<TObject>> configureValidator)
    {
        return new(
            path,
            async () =>
            {
                var builder = WebApplication.CreateSlimBuilder(
                    new WebApplicationOptions
                    {
                        ApplicationName = typeof(TObject).Name,
                    }
                );
                builder.WebHost.UseTestServer();
                builder.Services.AddOpenApi();
                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddFluentValidationOpenApi();
                var validator = new InlineValidator<TObject>();
                configureValidator(validator);
                builder.Services.AddSingleton<IValidator<TObject>>(validator);

#pragma warning disable CA2007
                await using var app = builder.Build();

                app.MapOpenApi();

                app.MapPost(path, (TObject container) => container);

                await app.StartAsync().ConfigureAwait(false);
                var testServer = (TestServer)app.Services.GetRequiredService<IServer>();
                var _client = testServer.CreateClient();

                var response = await _client.GetAsync("/openapi/v1.json");
                response.EnsureSuccessStatusCode();
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                return json["components"].ToString();
#pragma warning restore CA2007
            }
        );
    }

    [GeneratedRegex("^[a-zA-Z0-9]*$")]
    private static partial Regex MyRegex();
    [GeneratedRegex("^[a-zA-Z0-9]*$")]
    private static partial Regex MyRegex1();
}
