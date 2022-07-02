using Microsoft.CodeAnalysis;

#pragma warning disable 1591
#pragma warning disable CA1801

namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

internal class RestfulApiParameterMatcher : IRestfulApiParameterMatcher
{
    public RestfulApiParameterMatcher(
        Index parameterIndex,
        ApiConventionNameMatchBehavior nameMatch,
        string[] names,
        ApiConventionTypeMatchBehavior typeMatch,
        INamedTypeSymbol? type
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
    public INamedTypeSymbol? Type { get; }

    public bool IsMatch(ActionModel actionModel)
    {
        var parameters = actionModel.ActionMethod.Parameters;
        var offset = ParameterIndex.GetOffset(parameters.Length);
        if (offset >= 0 && offset < parameters.Length)
        {
            var parameter = parameters[ParameterIndex];
            if (TypeMatch == ApiConventionTypeMatchBehavior.AssignableFrom && Type != null)
            {
                if (Type.IsGenericType)
                {
                    static bool CheckType(ITypeSymbol symbol, INamedTypeSymbol type)
                    {
                        if (symbol is not INamedTypeSymbol namedTypeSymbol)
                        {
                            return false;
                        }

                        return namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, type);
                    }

                    var parameterIs = CheckType(parameter.Type, Type);
                    if (!parameterIs)
                    {
                        parameterIs = parameter.Type.AllInterfaces.Any(inter => CheckType(inter, Type));
                    }

                    if (!parameterIs)
                    {
                        return false;
                    }
                }
                else if (!actionModel.Compilation.HasImplicitConversion(parameter.Type, Type))
                {
                    return false;
                }
            }

            return NameMatch switch
            {
                ApiConventionNameMatchBehavior.Exact  => Names.Any(name => parameter.Name.Equals(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Prefix => Names.Any(name => parameter.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)),
                ApiConventionNameMatchBehavior.Suffix => Names.Any(name => parameter.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase)),
                _                                     => true
            };
        }

        return false;
    }
}

// IApiDescriptionProvider
