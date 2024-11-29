using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class OperationIdFilter : IOpenApiOperationTransformer
{
    /// <summary>
    ///     By default, pascalize converts strings to UpperCamelCase also removing underscores
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Pascalize(string input)
    {
        return Regex.Replace(input, "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Same as Pascalize except that the first character is lower case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Camelize(string input)
    {
        var word = Pascalize(input);
        #pragma warning disable CA1308
        return word.Length > 0 ? string.Concat(word.Substring(0, 1).ToLower(CultureInfo.InvariantCulture), word.AsSpan(1)) : word;
        #pragma warning restore CA1308
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(operation.OperationId) && context.Description.ActionDescriptor is ControllerActionDescriptor cad)
            operation.OperationId = cad.ActionName;

        if (operation.Parameters is null) return Task.CompletedTask;
        foreach (var parameter in operation.Parameters)
        {
            parameter.Name = Camelize(parameter.Name);
        }
        return Task.CompletedTask;
    }
}
