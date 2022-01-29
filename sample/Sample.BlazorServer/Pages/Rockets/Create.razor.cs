using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Create : ComponentBase
{
    public CreateRocket.Request Model { get; set; } = new CreateRocket.Request();

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IMediator Mediator { get; set; }

    public async Task Save()
    {
        await Mediator.Send(Model);
        NavigationManager.NavigateTo("/rockets");
    }
}
