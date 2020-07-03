using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

#pragma warning disable 1591
#pragma warning disable CA1801

namespace Rocket.Surgery.LaunchPad.Restful
{
    public enum RestfulApiMethod
    {
        List,
        Create,
        Read,
        Update,
        Delete
    }

    public interface IRestfulApiParameterMatcher
    {
        int ParameterIndex { get; }
        ApiConventionNameMatchBehavior NameMatch { get; }
        string[] Names { get; }
        ApiConventionTypeMatchBehavior TypeMatch { get; }
        Type? Type { get; }
        bool IsMatch(ActionModel actionModel);
    }

    public interface IRestfulApiMethodMatcher
    {
        RestfulApiMethod Method { get; }
        ApiConventionNameMatchBehavior NameMatch { get; }
        string[] Names { get; }
        IDictionary<int, IRestfulApiParameterMatcher> Parameters { get; }
        bool IsMatch(ActionModel actionModel);
    }

    class RestfulApiParameterMatcher : IRestfulApiParameterMatcher
    {
        public RestfulApiParameterMatcher(
            int parameterIndex,
            ApiConventionNameMatchBehavior nameMatch,
            string[] names,
            ApiConventionTypeMatchBehavior typeMatch,
            Type? type
        )
        {
            ParameterIndex = parameterIndex;
            NameMatch = nameMatch;
            Names = names;
            TypeMatch = typeMatch;
            Type = type;
        }

        public int ParameterIndex { get; }
        public ApiConventionNameMatchBehavior NameMatch { get; }
        public string[] Names { get; }
        public ApiConventionTypeMatchBehavior TypeMatch { get; }
        public Type? Type { get; }

        public bool IsMatch(ActionModel actionModel)
        {
            var parameter = actionModel.ActionMethod.GetParameters()[ParameterIndex];
            if (TypeMatch == ApiConventionTypeMatchBehavior.AssignableFrom && Type != null)
            {
                if (!Type.IsAssignableFrom(parameter.ParameterType))
                {
                    return false;
                }
            }

            return NameMatch switch
            {
                ApiConventionNameMatchBehavior.Exact  => Names.Any(name => parameter.Name!.Equals(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Prefix => Names.Any(name => parameter.Name!.StartsWith(name!, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Suffix => Names.Any(name => parameter.Name!.EndsWith(name!, StringComparison.OrdinalIgnoreCase)),
                _                                     => true
            };
        }
    }

    public class RestfulApiMethodBuilder : IRestfulApiMethodMatcher
    {
        public RestfulApiMethod Method { get; }
        private ApiConventionNameMatchBehavior _nameMatchBehavior = ApiConventionNameMatchBehavior.Any;
        private string[] _names = Array.Empty<string>();

        private readonly IDictionary<int, IRestfulApiParameterMatcher> _parameters = new Dictionary<int, IRestfulApiParameterMatcher>();

        public RestfulApiMethodBuilder(RestfulApiMethod method) => Method = method;

        public RestfulApiMethodBuilder MatchSuffix(string value, params string[] values)
        {
            _names = new[] { value }.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Suffix;

            return this;
        }

        public RestfulApiMethodBuilder MatchPrefix(string value, params string[] values)
        {
            _names = new[] { value }.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Prefix;

            return this;
        }

        public RestfulApiMethodBuilder MatchName(string value, params string[] values)
        {
            _names = new[] { value }.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Exact;

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterName(int parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = new RestfulApiParameterMatcher(
                    parameter,
                    ApiConventionNameMatchBehavior.Any,
                    null,
                    ApiConventionTypeMatchBehavior.Any,
                    null
                );
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Exact,
                new[] { value }.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder HasParameter(int parameter)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = new RestfulApiParameterMatcher(
                    parameter,
                    ApiConventionNameMatchBehavior.Any,
                    Array.Empty<string>(),
                    ApiConventionTypeMatchBehavior.Any,
                    null
                );
            }

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterPrefix(int parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = new RestfulApiParameterMatcher(
                    parameter,
                    ApiConventionNameMatchBehavior.Any,
                    null,
                    ApiConventionTypeMatchBehavior.Any,
                    null
                );
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Prefix,
                new[] { value }.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterSuffix(int parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = new RestfulApiParameterMatcher(
                    parameter,
                    ApiConventionNameMatchBehavior.Any,
                    null,
                    ApiConventionTypeMatchBehavior.Any,
                    null
                );
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Suffix,
                new[] { value }.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterType(int parameter, Type type)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = new RestfulApiParameterMatcher(
                    parameter,
                    ApiConventionNameMatchBehavior.Any,
                    null,
                    ApiConventionTypeMatchBehavior.Any,
                    null
                );
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(parameter, item.NameMatch, item.Names, ApiConventionTypeMatchBehavior.AssignableFrom, type);

            return this;
        }

        public bool IsValid() => _names.Length > 0;
        public ApiConventionNameMatchBehavior NameMatch => _nameMatchBehavior;
        public string[] Names => _names;
        public IDictionary<int, IRestfulApiParameterMatcher> Parameters => _parameters;

        public bool IsMatch(ActionModel actionModel)
        {
            var nameMatch = NameMatch switch
            {
                ApiConventionNameMatchBehavior.Exact  => Names.Any(name => actionModel.ActionName!.Equals(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Prefix => Names.Any(name => actionModel.ActionName!.StartsWith(name!, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Suffix => Names.Any(name => actionModel.ActionName!.EndsWith(name!, StringComparison.OrdinalIgnoreCase)),
                _                                     => true
            };

            if (!nameMatch)
                return false;

            if (actionModel.ActionMethod.GetParameters().Length >= _parameters.Count)
            {
                return _parameters.Values.All(z => z.IsMatch(actionModel));
            }

            return false;
        }
    }

    public class RestfulApiOptions
    {
        private static readonly RestfulApiMethodBuilder[] DefaultBuilders = new[]
        {
            new RestfulApiMethodBuilder(RestfulApiMethod.List)
               .MatchPrefix("List")
               .MatchParameterType(0, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Read)
               .MatchPrefix("Get", "Find", "Fetch", "Read")
               .MatchParameterType(0, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Create)
               .MatchPrefix("Post", "Create", "Add")
               .MatchParameterType(0, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Update)
               .MatchPrefix("Put", "Edit", "Update")
               .MatchParameterSuffix(0, "id")
               .MatchParameterType(1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Update)
               .MatchPrefix("Put", "Edit", "Update")
               .MatchParameterSuffix(0, "id")
               .MatchParameterSuffix(1, "model"),
            new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
               .MatchPrefix("Delete", "Remove")
               .MatchParameterType(0, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
               .MatchPrefix("Delete", "Remove")
               .MatchParameterSuffix(0, "id"),
        };

        private readonly List<RestfulApiMethodBuilder> _builders = DefaultBuilders.ToList();

        public RestfulApiOptions() { }

        public RestfulApiMethodBuilder AddMethod(RestfulApiMethod method)
        {
            var builder = new RestfulApiMethodBuilder(method);
            _builders.Add(builder);
            return builder;
        }

        internal ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> GetMatchers() => _builders.Where(z => z.IsValid())
           .OfType<IRestfulApiMethodMatcher>()
           .ToLookup(x => x.Method);

        public IDictionary<RestfulApiMethod, int> MethodStatusCodeMap = new Dictionary<RestfulApiMethod, int>()
        {
            [RestfulApiMethod.List] = StatusCodes.Status200OK,
            [RestfulApiMethod.Read] = StatusCodes.Status200OK,
            [RestfulApiMethod.Create] = StatusCodes.Status201Created,
            [RestfulApiMethod.Update] = StatusCodes.Status200OK,
            [RestfulApiMethod.Delete] = StatusCodes.Status204NoContent,
        };

        public IValidationActionResultFactory ValidationActionResultFactory { get; set; } = new UnprocessableEntityActionResultFactory();
    }

    public interface IValidationActionResultFactory
    {
        ActionResult CreateActionResult(ValidationProblemDetails problemDetails);
        int StatusCode { get; }
    }

    public class UnprocessableEntityActionResultFactory : IValidationActionResultFactory
    {
        public ActionResult CreateActionResult(ValidationProblemDetails problemDetails)
        {
            problemDetails.Status = StatusCode;
            return new UnprocessableEntityObjectResult(problemDetails);
        }

        public int StatusCode { get; } = StatusCodes.Status422UnprocessableEntity;
    }

    public class RestfulApiApplicationModelProvider : IApplicationModelProvider
    {
        private readonly IOptions<RestfulApiOptions> _options;

        public RestfulApiApplicationModelProvider(IOptions<RestfulApiOptions> options)
        {
            _options = options;
            ActionModelConventions = new List<IActionModelConvention>()
            {
                new RestfulApiActionModelConvention(options)
            };
        }

        public List<IActionModelConvention> ActionModelConventions { get; }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            foreach (var controller in context.Result.Controllers)
            {
                if (!typeof(RestfulApiController).IsAssignableFrom(controller.ControllerType))
                {
                    return;
                }

                foreach (var action in controller.Actions)
                {
                    foreach (var convention in ActionModelConventions)
                    {
                        convention.Apply(action);
                    }
                }
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context) { }

        public int Order => -1000;
    }

    // IApiDescriptionProvider

    public class RestfulApiActionModelConvention : IActionModelConvention
    {
        private readonly ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> _matchers;
        private readonly RestfulApiOptions _options;


        public RestfulApiActionModelConvention(IOptions<RestfulApiOptions> options)
        {
            _matchers = options.Value.GetMatchers();
            _options = options.Value;
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
               .Any(z => z is ProducesDefaultResponseTypeAttribute);

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

            if (!providerLookup[StatusCodes.Status404NotFound].Any())
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
    }
}