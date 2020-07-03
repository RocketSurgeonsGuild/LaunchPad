using System;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Rocket.Surgery.LaunchPad.Restful.Problems
{
    public class ProblemDetailsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.UseProblemDetails();
            next(app);
        };
    }
}