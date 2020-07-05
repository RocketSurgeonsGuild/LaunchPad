using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

#pragma warning disable 1591
#pragma warning disable CA1801

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
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

    // IApiDescriptionProvider
}