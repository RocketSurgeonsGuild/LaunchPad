using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal class RestfulApiActionModelConvention : IActionModelConvention
{
    private static string? GetHttpMethod(ActionModel action)
    {
        var httpMethods = action.Attributes
                                .OfType<IActionHttpMethodProvider>()
                                .SelectMany(a => a.HttpMethods)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToArray();
        // Not valid for actions with more than one verb
        if (httpMethods.Length > 1)
        {
            return null;
        }

        return httpMethods[0];
    }

    private readonly ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> _matchers;
    private readonly RestfulApiOptions _options;


    public RestfulApiActionModelConvention(IOptions<RestfulApiOptions> options)
    {
        _matchers = options.Value.GetMatchers();
        _options = options.Value;
    }

    private void UpdateProviders(
        ActionModel actionModel
    )
    {
        var providerLookup = actionModel.Filters.OfType<IApiResponseMetadataProvider>()
                                        .ToLookup(x => x.StatusCode);

        var hasSuccess = providerLookup.Any(z => z.Key >= 200 && z.Key < 300);
        var match = _matchers
                   .SelectMany(z => z)
                   .FirstOrDefault(x => x.IsMatch(actionModel));
        var hasDefault = providerLookup
                        .SelectMany(z => z)
                        .Any(z => z is IApiDefaultResponseMetadataProvider);

        if (!hasDefault)
        {
            actionModel.Filters.Add(new ProducesDefaultResponseTypeAttribute());
        }

        if (!hasSuccess)
        {
            if (actionModel.ActionMethod.ReturnType == typeof(Task<ActionResult>))
            {
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent));
            }
            else if (match != null)
            {
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(_options.MethodStatusCodeMap[match.Method]));
            }
            else
            {
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
            }
        }

        if (!providerLookup[StatusCodes.Status404NotFound].Any() && match?.Method != RestfulApiMethod.List)
        {
            actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
        }

        if (!providerLookup[StatusCodes.Status400BadRequest].Any())
        {
            actionModel.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status400BadRequest));
        }

        if (!providerLookup[_options.ValidationActionResultFactory.StatusCode].Any())
        {
            actionModel.Filters.Add(
                new ProducesResponseTypeAttribute(typeof(FluentValidationProblemDetails), _options.ValidationActionResultFactory.StatusCode)
            );
        }
    }

    public void Apply(ActionModel action)
    {
        if (!typeof(RestfulApiController).IsAssignableFrom(action.Controller.ControllerType))
        {
            return;
        }

        var httpMethod = GetHttpMethod(action);
        if (string.IsNullOrWhiteSpace(httpMethod))
            return;

        UpdateProviders(action);
    }
}
