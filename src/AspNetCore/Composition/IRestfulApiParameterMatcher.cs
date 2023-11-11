using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal interface IRestfulApiParameterMatcher
{
    Index ParameterIndex { get; }
    ApiConventionNameMatchBehavior NameMatch { get; }
    string[] Names { get; }
    ApiConventionTypeMatchBehavior TypeMatch { get; }
    Type? Type { get; }

    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    bool IsMatch(ActionModel actionModel);
}
