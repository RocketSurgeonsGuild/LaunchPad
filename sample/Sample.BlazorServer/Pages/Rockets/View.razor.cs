using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class View : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    public RocketModel Model { get; set; } = null!;
    [Inject] private IMediator Mediator { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = await Mediator.Send(new GetRocket.Request { Id = Id });
    }
}
