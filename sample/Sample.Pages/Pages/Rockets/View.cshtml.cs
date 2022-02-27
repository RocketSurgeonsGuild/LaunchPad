using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketViewModel : MediatorPageModel
{
    [UsedImplicitly]
    [BindProperty(SupportsGet = true)]
    public RocketId Id { get; set; }

    public RocketModel Rocket { get; set; } = null!;

    public virtual async Task OnGet()
    {
        Rocket = await Send(new GetRocket.Request { Id = Id });
    }
}
