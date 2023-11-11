using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal interface IRestfulApiMethodMatcher
{
    RestfulApiMethod Method { get; }
    ApiConventionNameMatchBehavior NameMatch { get; }
    string[] Names { get; }
    IDictionary<Index, IRestfulApiParameterMatcher> Parameters { get; }

    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    bool IsMatch(ActionModel actionModel);
}
