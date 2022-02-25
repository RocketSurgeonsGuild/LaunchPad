using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Index : ComponentBase
{
    public IEnumerable<RocketModel> Rockets { get; set; } = null!;

    [Inject] private IMediator Mediator { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Rockets = await Mediator.CreateStream(new ListRockets.Request(null)).ToListAsync();
    }
}
