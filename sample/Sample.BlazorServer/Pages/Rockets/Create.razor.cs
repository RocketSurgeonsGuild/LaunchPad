using Microsoft.AspNetCore.Components;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Create : ComponentBase
{
    public CreateRocket.Request Model { get; set; } = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private IMediator Mediator { get; set; } = null!;

    public async Task Save()
    {
        ( await Mediator.Send(Model) ).Value.Expect<CreateRocket.Response>();
        NavigationManager.NavigateTo("/rockets");
    }
}
