using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Edit : ComponentBase
{
    [Parameter]
    public RocketId Id { get; set; }

    public EditRocket.Request Model { get; set; } = new();

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IMediator Mediator { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = EditRocket.MapRequest(await Mediator.Send(new GetRocket.Request { Id = Id, }));
    }

    public async Task Save()
    {
        await Mediator.Send(Model with { Id = Id, });
        NavigationManager.NavigateTo("/rockets");
    }
}