using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sample.Classic.Restful.Tests;

/// <summary>What every action documents: generated ones a success response, and a 400 whenever they bind a value from the request.</summary>
public class ClassicRestfulConventionTests
{
    public static IEnumerable<Func<ApiDescriptionData>> ApiDescriptions() => ApiDescriptionDataSource.Rows();

    [Test]
    [MethodDataSource(nameof(ApiDescriptions))]
    public async Task Should_Have_Success_Response_Types(ApiDescriptionData description) => await description.Description.SupportedResponseTypes.Should().Contain(z => z.StatusCode is >= 200 and < 300);

    [Test]
    [MethodDataSource(nameof(ApiDescriptions))]
    public async Task Should_Have_Bad_Request_Responses_When_Binding(ApiDescriptionData description)
    {
        var bound = description.Description.ParameterDescriptions.Any(p => p.Source == BindingSource.Path || p.Source == BindingSource.Query || p.Source == BindingSource.Body);
        if (!bound) return;
        await description.Description.SupportedResponseTypes.Should().Contain(z => z.StatusCode == StatusCodes.Status400BadRequest);
    }
}
