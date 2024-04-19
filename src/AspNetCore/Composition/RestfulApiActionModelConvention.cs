using System.Collections.Concurrent;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal class RestfulApiActionModelConvention : IActionModelConvention, ISchemaFilter
{
    private static string? GetHttpMethod(ActionModel action)
    {
        var httpMethods = action.Attributes
                                .OfType<IActionHttpMethodProvider>()
                                .SelectMany(a => a.HttpMethods)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToArray();
        // Not valid for actions with more than one verb
        if (httpMethods.Length > 1) return null;

        return httpMethods[0];
    }

    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    private static void ExtractParameterDetails(ActionModel action)
    {
        var requestParameter = action.Parameters.FirstOrDefault(
                z => z.ParameterInfo.ParameterType.GetInterfaces().Any(
                    i => i.IsGenericType &&
                         ( typeof(IRequest<>) == i.GetGenericTypeDefinition()
                        || typeof(IStreamRequest<>) == i.GetGenericTypeDefinition() )
                )
            )
            ;
        if (requestParameter is null) return;

        // Likely hit the generator, no point running from here
        if (requestParameter.Attributes.Count(z => z is BindAttribute or CustomizeValidatorAttribute) == 2)
        {
            _propertiesToHideFromOpenApi.TryAdd(
                requestParameter.ParameterType,
                requestParameter.ParameterType.GetProperties().Select(z => z.Name).Except(
                    requestParameter.Attributes.OfType<BindAttribute>().SelectMany(z => z.Include)
                ).ToArray()
            );
            return;
        }

        var index = action.Parameters.IndexOf(requestParameter);
        var newAttributes = requestParameter.Attributes.ToList();
        var otherParams = action.Parameters
                                .Except(new[] { requestParameter })
                                .Select(z => z.ParameterName)
                                .ToArray();

        var propertyAndFieldNames = requestParameter.ParameterType
                                                    .GetProperties()
                                                    .Select(z => z.Name)
                                                    .ToArray();
        var bindNames = propertyAndFieldNames
                       .Except(otherParams, StringComparer.OrdinalIgnoreCase)
                       .ToArray();

        if (propertyAndFieldNames.Length != bindNames.Length)
        {
            var ignoreBindings = propertyAndFieldNames.Except(bindNames, StringComparer.OrdinalIgnoreCase).ToArray();
            newAttributes.Add(new BindAttribute(bindNames));
            action.Properties.Add(
                typeof(CustomizeValidatorAttribute),
                bindNames
            );
            _propertiesToHideFromOpenApi.TryAdd(requestParameter.ParameterType, ignoreBindings);

            var model = action.Parameters[index] = new ParameterModel(requestParameter.ParameterInfo, newAttributes)
            {
                Action = requestParameter.Action,
                BindingInfo = requestParameter.BindingInfo,
                ParameterName = requestParameter.ParameterName
            };
            foreach (var item in requestParameter.Properties)
            {
                model.Properties.Add(item);
            }
        }
    }

    private static readonly ConcurrentDictionary<Type, string[]> _propertiesToHideFromOpenApi = new();

    private readonly ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> _matchers;
    private readonly RestfulApiOptions _options;

    /// <summary>
    /// An action model convention that allows <see cref="IRequest{TResponse}"/> and <see cref="IStreamRequest{TResponse}"/> to be used as parameters to controller actions.
    /// </summary>
    /// <param name="options"></param>
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

        if (!providerLookup[StatusCodes.Status404NotFound].Any() && match?.Method != RestfulApiMethod.List) actionModel.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));

        if (!providerLookup[StatusCodes.Status400BadRequest].Any()) actionModel.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status400BadRequest));

        if (!providerLookup[_options.ValidationStatusCode].Any())
            actionModel.Filters.Add(
                new ProducesResponseTypeAttribute(typeof(FluentValidationProblemDetails), _options.ValidationStatusCode)
            );
    }

    // TODO: Make a source generator for this to work without generics
    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    public void Apply(ActionModel action)
    {
        if (!typeof(RestfulApiController).IsAssignableFrom(action.Controller.ControllerType)) return;

        var httpMethod = GetHttpMethod(action);
        if (string.IsNullOrWhiteSpace(httpMethod))
            return;

        UpdateProviders(action);
        ExtractParameterDetails(action);
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (_propertiesToHideFromOpenApi.TryGetValue(context.Type, out var propertiesToRemove))
            foreach (var property in propertiesToRemove
                                    .Join(schema.Properties, z => z, z => z.Key, (_, b) => b.Key, StringComparer.OrdinalIgnoreCase)
                                    .ToArray())
            {
                schema.Properties.Remove(property);
            }
    }
}
