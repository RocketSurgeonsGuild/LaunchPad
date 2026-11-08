using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

/// <summary>
///     An action model convention that documents the response types of <see cref="RestfulApiController" /> actions from their name and shape.
/// </summary>
/// <param name="options"></param>
internal class RestfulApiActionModelConvention(IOptions<RestfulApiOptions> options) : IActionModelConvention
{
    private static string? GetHttpMethod(ActionModel action)
    {
        var httpMethods = action
                         .Attributes
                         .OfType<IActionHttpMethodProvider>()
                         .SelectMany(a => a.HttpMethods)
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .ToArray();
        // Not valid for actions with more than one verb
        return httpMethods.Length > 1 ? null : httpMethods[0];
    }

    private readonly ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> _matchers = options.Value.GetMatchers();
    private readonly RestfulApiOptions _options = options.Value;

    private void UpdateProviders(
        ActionModel actionModel
    )
    {
        var providerLookup = actionModel
                            .Filters.OfType<IApiResponseMetadataProvider>()
                            .ToLookup(x => x.StatusCode);

        var hasSuccess = providerLookup.Any(z => z.Key is >= 200 and < 300);
        var match = _matchers
                   .SelectMany(z => z)
                   .FirstOrDefault(x => x.IsMatch(actionModel));
        var hasDefault = providerLookup
                        .SelectMany(z => z)
                        .Any(z => z is IApiDefaultResponseMetadataProvider);

        if (!hasDefault) actionModel.Filters.Add(new ProducesDefaultResponseTypeAttribute());

        if (!hasSuccess)
        {
            if (actionModel.ActionMethod.ReturnType == typeof(Task<ActionResult>))
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent));
            else if (match != null)
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(_options.MethodStatusCodeMap[match.Method]));
            else
                actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
        }

        if (!providerLookup[StatusCodes.Status404NotFound].Any() && match?.Method != RestfulApiMethod.List)
            actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));

        if (!providerLookup[StatusCodes.Status400BadRequest].Any())
            actionModel.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status400BadRequest));

        if (!providerLookup[_options.ValidationStatusCode].Any())
        {
            actionModel.Filters.Add(
                new ProducesResponseTypeAttribute(_options.ValidationProblemDetailsType, _options.ValidationStatusCode)
            );
        }
    }

    // TODO: Make a source generator for this to work without generics
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public void Apply(ActionModel action)
    {
        if (!typeof(RestfulApiController).IsAssignableFrom(action.Controller.ControllerType)) return;

        var httpMethod = GetHttpMethod(action);
        if (string.IsNullOrWhiteSpace(httpMethod))
            return;

        UpdateProviders(action);
    }
}
