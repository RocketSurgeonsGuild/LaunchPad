using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal class OperationIdFilter : IOperationFilter
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
        return word.Length > 0 ? word.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + word.Substring(1) : word;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(operation.OperationId) &&
            context.ApiDescription.ActionDescriptor is ControllerActionDescriptor cad)
        {
            operation.OperationId = cad.ActionName;
        }

        foreach (var parameter in operation.Parameters)
        {
            parameter.Name = Camelize(parameter.Name);
        }
    }
}
