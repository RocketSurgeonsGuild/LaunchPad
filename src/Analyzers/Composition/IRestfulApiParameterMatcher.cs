using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

internal interface IRestfulApiParameterMatcher
{
    Index ParameterIndex { get; }
    ApiConventionNameMatchBehavior NameMatch { get; }
    string[] Names { get; }
    ApiConventionTypeMatchBehavior TypeMatch { get; }
    INamedTypeSymbol? Type { get; }
    bool IsMatch(ActionModel actionModel);
}
