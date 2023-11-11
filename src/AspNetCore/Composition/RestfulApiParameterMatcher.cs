using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

#pragma warning disable 1591
#pragma warning disable CA1801

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal class RestfulApiParameterMatcher(
    Index parameterIndex,
    ApiConventionNameMatchBehavior nameMatch,
    string[] names,
    ApiConventionTypeMatchBehavior typeMatch,
    Type? type
)
    : IRestfulApiParameterMatcher
{
    public Index ParameterIndex { get; } = parameterIndex;
    public ApiConventionNameMatchBehavior NameMatch { get; } = nameMatch;
    public string[] Names { get; } = names;
    public ApiConventionTypeMatchBehavior TypeMatch { get; } = typeMatch;
    public Type? Type { get; } = type;

    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    public bool IsMatch(ActionModel actionModel)
    {
        var parameters = actionModel.ActionMethod.GetParameters();
        var offset = ParameterIndex.GetOffset(parameters.Length);
        if (offset >= 0 && offset < parameters.Length)
        {
            var parameter = parameters[ParameterIndex];
            if (TypeMatch == ApiConventionTypeMatchBehavior.AssignableFrom && Type != null)
            {
                if (Type.IsGenericTypeDefinition)
                {
                    var parameterIs = parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == Type;
                    if (!parameterIs)
                        parameterIs = parameter.ParameterType.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == Type);
                    if (!parameterIs)
                    {
                        return false;
                    }
                }
                else if (!Type.IsAssignableFrom(parameter.ParameterType))
                {
                    return false;
                }
            }

            // ReSharper disable NullableWarningSuppressionIsUsed
            return NameMatch switch
            {
                ApiConventionNameMatchBehavior.Exact  => Names.Any(name => parameter.Name!.Equals(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Prefix => Names.Any(name => parameter.Name!.StartsWith(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Suffix => Names.Any(name => parameter.Name!.EndsWith(name, StringComparison.OrdinalIgnoreCase)),
                _                                     => true
            };
            // ReSharper enable NullableWarningSuppressionIsUsed
        }

        return false;
    }
}

// IApiDescriptionProvider
