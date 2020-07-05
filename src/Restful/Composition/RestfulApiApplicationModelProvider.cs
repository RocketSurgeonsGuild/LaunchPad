using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Restful.Composition
{
    class RestfulApiApplicationModelProvider : IApplicationModelProvider
    {
        public RestfulApiApplicationModelProvider(IOptions<RestfulApiOptions> options)
        {
            ActionModelConventions = new List<IActionModelConvention>()
            {
                new RestfulApiActionModelConvention(options)
            };
        }

        public List<IActionModelConvention> ActionModelConventions { get; }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            foreach (var controller in context.Result.Controllers)
            {
                if (!typeof(RestfulApiController).IsAssignableFrom(controller.ControllerType))
                {
                    return;
                }

                foreach (var action in controller.Actions)
                {
                    foreach (var convention in ActionModelConventions)
                    {
                        convention.Apply(action);
                    }
                }
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context) { }

        public int Order => -1000;
    }
}