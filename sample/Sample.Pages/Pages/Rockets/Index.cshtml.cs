using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketIndexModel(IMediator mediator) : PageModel
{
    // [BindProperty]
    // public Movie Movie { get; set; }
    [UsedImplicitly] public IEnumerable<RocketModel> Rockets { get; set; } = null!;

    public async Task OnGet()
    {
        Rockets = await mediator.CreateStream(new ListRockets.Request(null)).ToListAsync(HttpContext.RequestAborted);
    }
}
