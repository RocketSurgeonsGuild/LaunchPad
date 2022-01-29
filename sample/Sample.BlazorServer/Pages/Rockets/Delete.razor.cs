using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Delete : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    public RocketModel Model { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IMediator Mediator { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await Mediator.Send(new GetRocket.Request { Id = Id });
    }

    public async Task Save()
    {
        await Mediator.Send(new DeleteRocket.Request { Id = Id });
        NavigationManager.NavigateTo("/rockets");
    }
}
