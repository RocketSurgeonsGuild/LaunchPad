using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

using ZeroAlloc.Mediator;

namespace Sample.Pages.Pages.Rockets;

public class RocketViewModel : PageModel
{
    private protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    [UsedImplicitly]
    [BindProperty(SupportsGet = true)]
    public RocketId Id { get; set; }

    public RocketModel Rocket { get; set; } = null!;

    public virtual async Task OnGet() => Rocket = await Mediator.Send(new GetRocket.Request(Id), HttpContext.RequestAborted);
}
