using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition
{
    public interface IRestfulApiMethodMatcher
    {
        RestfulApiMethod Method { get; }
        ApiConventionNameMatchBehavior NameMatch { get; }
        string[] Names { get; }
        IDictionary<Index, IRestfulApiParameterMatcher> Parameters { get; }
        bool IsMatch(ActionModel actionModel);
    }
}