using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
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
}