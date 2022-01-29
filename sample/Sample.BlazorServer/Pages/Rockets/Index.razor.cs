using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Index : ComponentBase
{
    public IEnumerable<RocketModel> Rockets { get; set; }

    [Inject] private IMediator Mediator { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Rockets = await Mediator.Send(new ListRockets.Request());
    }
}
