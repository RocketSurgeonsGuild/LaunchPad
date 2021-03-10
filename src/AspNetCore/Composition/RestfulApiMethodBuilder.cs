using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition
{
    public class RestfulApiMethodBuilder : IRestfulApiMethodMatcher
    {
        public RestfulApiMethod Method { get; }
        private int? _parameterCount;
        private ApiConventionNameMatchBehavior _nameMatchBehavior = ApiConventionNameMatchBehavior.Any;
        private string[] _names = Array.Empty<string>();

        private readonly IDictionary<Index, IRestfulApiParameterMatcher> _parameters = new Dictionary<Index, IRestfulApiParameterMatcher>();

        public RestfulApiMethodBuilder(RestfulApiMethod method) => Method = method;

        public RestfulApiMethodBuilder MatchSuffix(string value, params string[] values)
        {
            _names = new[] {value}.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Suffix;

            return this;
        }

        public RestfulApiMethodBuilder MatchPrefix(string value, params string[] values)
        {
            _names = new[] {value}.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Prefix;

            return this;
        }

        public RestfulApiMethodBuilder MatchName(string value, params string[] values)
        {
            _names = new[] {value}.Concat(values).ToArray();
            _nameMatchBehavior = ApiConventionNameMatchBehavior.Exact;

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterName(Index parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = defaultMatcher(parameter);
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Exact,
                new[] {value}.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder HasParameter(Index parameter)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = defaultMatcher(parameter);
            }

            _parameters[parameter] =
                new RestfulApiParameterMatcher(
                    parameter,
                    item.NameMatch,
                    item.Names?.Length > 0 ? item.Names : Array.Empty<string>(),
                    item.TypeMatch,
                    item.Type
                );

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterCount(int count)
        {
            _parameterCount = count;
            return this;
        }

        public RestfulApiMethodBuilder MatchParameterPrefix(Index parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = defaultMatcher(parameter);
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Prefix,
                new[] {value}.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterSuffix(Index parameter, string value, params string[] values)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = defaultMatcher(parameter);
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                ApiConventionNameMatchBehavior.Suffix,
                new[] {value}.Concat(values).ToArray(),
                item.TypeMatch,
                item.Type
            );

            return this;
        }

        public RestfulApiMethodBuilder MatchParameterType(Index parameter, Type type)
        {
            if (!_parameters.TryGetValue(parameter, out var item))
            {
                item = _parameters[parameter] = defaultMatcher(parameter);
            }

            _parameters[parameter] = new RestfulApiParameterMatcher(
                parameter,
                item.NameMatch,
                item.Names,
                ApiConventionTypeMatchBehavior.AssignableFrom,
                type
            );

            return this;
        }

        private static RestfulApiParameterMatcher defaultMatcher(Index index) => new RestfulApiParameterMatcher(
            index,
            ApiConventionNameMatchBehavior.Any,
            null,
            ApiConventionTypeMatchBehavior.Any,
            null
        );

        public bool IsValid() => _names.Length > 0;
        public ApiConventionNameMatchBehavior NameMatch => _nameMatchBehavior;
        public string[] Names => _names;
        public IDictionary<Index, IRestfulApiParameterMatcher> Parameters => _parameters;

        public bool IsMatch(ActionModel actionModel)
        {
            var nameMatch = NameMatch switch
            {
                ApiConventionNameMatchBehavior.Exact => Names.Any(name => actionModel.ActionName!.Equals(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Prefix => Names.Any(name => actionModel.ActionName!.StartsWith(name!, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Suffix => Names.Any(name => actionModel.ActionName!.EndsWith(name!, StringComparison.OrdinalIgnoreCase)),
                _ => true
            };

            if (!nameMatch)
                return false;

            var parameters = actionModel.ActionMethod.GetParameters();
            if (_parameterCount.HasValue && parameters.Length != _parameterCount.Value)
            {
                return false;
            }

            if (parameters.Length >= _parameters.Count)
            {
                return _parameters.Values.All(z => z.IsMatch(actionModel));
            }

            return false;
        }
    }
}