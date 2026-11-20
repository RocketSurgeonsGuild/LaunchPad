using Microsoft.AspNetCore.Components;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Models;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Edit : ComponentBase
{
    [Parameter]
    public RocketId Id { get; set; }

    public EditRocket.RequestViewModel Model { get; set; } = new();

    public Problem? Problem { get; private set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IMediator Mediator { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (( await Mediator.Send(new GetRocket.Request(Id)) ).Value.TryGetValue<RocketModel>(out var rocket, out var problem))
        {
            Model = EditRocket.RequestViewModel.From(EditRocket.MapRequest(rocket));
        }
        else
        {
            Problem = problem;
        }
    }

    public async Task Save()
    {
        if (!( await Mediator.Send(Model.ToImmutable() with { Id = Id, }) ).Value.TryGetValue<RocketModel>(out _, out var problem))
        {
            Problem = problem;
            return;
        }

        NavigationManager.NavigateTo("/rockets");
    }
}
