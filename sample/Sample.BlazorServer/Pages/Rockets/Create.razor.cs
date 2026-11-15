using Microsoft.AspNetCore.Components;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Create : ComponentBase
{
    public CreateRocket.Request Model { get; set; } = new();

    public Problem? Problem { get; private set; }

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private IMediator Mediator { get; set; } = null!;

    public async Task Save()
    {
        if (!( await Mediator.Send(Model) ).Value.TryGetValue<CreateRocket.Response>(out _, out var problem))
        {
            Problem = problem;
            return;
        }

        NavigationManager.NavigateTo("/rockets");
    }
}
