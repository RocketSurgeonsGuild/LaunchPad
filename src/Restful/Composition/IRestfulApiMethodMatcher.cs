using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
    public interface IRestfulApiMethodMatcher
    {
        RestfulApiMethod Method { get; }
        ApiConventionNameMatchBehavior NameMatch { get; }
        string[] Names { get; }
        IDictionary<int, IRestfulApiParameterMatcher> Parameters { get; }
        bool IsMatch(ActionModel actionModel);
    }
}