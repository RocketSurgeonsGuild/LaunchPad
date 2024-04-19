using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

internal class RestfulApiApplicationModelProvider(IOptions<RestfulApiOptions> options) : IApplicationModelProvider
{
    public List<IActionModelConvention> ActionModelConventions { get; } = new()
    {
        new RestfulApiActionModelConvention(options)
    };

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
        foreach (var controller in context.Result.Controllers)
        {
            if (!typeof(RestfulApiController).IsAssignableFrom(controller.ControllerType)) return;

            foreach (var action in controller.Actions)
            {
                foreach (var convention in ActionModelConventions)
                {
                    convention.Apply(action);
                }
            }
        }
    }

    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
    }

    public int Order => -1000;
}
