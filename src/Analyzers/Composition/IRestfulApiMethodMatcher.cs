namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

internal interface IRestfulApiMethodMatcher
{
    RestfulApiMethod Method { get; }
    ApiConventionNameMatchBehavior NameMatch { get; }
    string[] Names { get; }
    IDictionary<Index, IRestfulApiParameterMatcher> Parameters { get; }
    bool IsMatch(ActionModel actionModel);
}
