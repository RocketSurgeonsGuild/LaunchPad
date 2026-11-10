using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

using ZeroAlloc.Mediator;

namespace Sample.Pages.Pages.Rockets;

public class RocketIndexModel : PageModel
{
    private IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    // [BindProperty]
    // public Movie Movie { get; set; }
    [UsedImplicitly] public IEnumerable<RocketModel> Rockets { get; set; } = null!;

    public async Task OnGet() => Rockets = await Mediator.CreateStream(new ListRockets.Request(null), HttpContext.RequestAborted).ToListAsync(HttpContext.RequestAborted);
}
