using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

#pragma warning disable 1591
#pragma warning disable CA1801

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal class RestfulApiParameterMatcher : IRestfulApiParameterMatcher
{
    public RestfulApiParameterMatcher(
        Index parameterIndex,
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

    public Index ParameterIndex { get; }
    public ApiConventionNameMatchBehavior NameMatch { get; }
    public string[] Names { get; }
    public ApiConventionTypeMatchBehavior TypeMatch { get; }
    public Type? Type { get; }

    public bool IsMatch(ActionModel actionModel)
    {
        var parameters = actionModel.ActionMethod.GetParameters();
        var offset = ParameterIndex.GetOffset(parameters.Length);
        if (offset >= 0 && offset < parameters.Length)
        {
            var parameter = parameters[ParameterIndex];
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

        return false;
    }
}

// IApiDescriptionProvider
