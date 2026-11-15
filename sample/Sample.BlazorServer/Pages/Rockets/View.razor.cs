using Microsoft.AspNetCore.Components;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Models;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class View : ComponentBase
{
    [Parameter] public RocketId Id { get; set; }

    public RocketModel? Model { get; set; }

    public Problem? Problem { get; private set; }
    [Inject] private IMediator Mediator { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (( await Mediator.Send(new GetRocket.Request(Id)) ).Value.TryGetValue<RocketModel>(out var model, out var problem)) Model = model;
        else Problem = problem;
    }
}
