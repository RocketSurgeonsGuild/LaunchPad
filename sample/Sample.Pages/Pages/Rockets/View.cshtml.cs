using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Models;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;
// PageModel has NotFound(), Unauthorized() and Conflict() methods, and Rocket is a property here, so the problem cases need aliases
using ConflictProblem = Rocket.Surgery.LaunchPad.Foundation.Conflict;
using NotFoundProblem = Rocket.Surgery.LaunchPad.Foundation.NotFound;
using UnauthorizedProblem = Rocket.Surgery.LaunchPad.Foundation.Unauthorized;

namespace Sample.Pages.Pages.Rockets;

public class RocketViewModel : PageModel
{
    private protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    [UsedImplicitly]
    [BindProperty(SupportsGet = true)]
    public RocketId Id { get; set; }

    public RocketModel Rocket { get; set; } = null!;

    public virtual async Task<IActionResult> OnGet()
    {
        if (!( await Mediator.Send(new GetRocket.Request(Id), HttpContext.RequestAborted) ).Value.TryGetValue<RocketModel>(out var rocket, out var problem))
        {
            return ProblemResult(problem);
        }

        Rocket = rocket;
        return Page();
    }

    /// <summary>Answers a problem that leaves nothing to show with its HTTP status.</summary>
    private protected IActionResult ProblemResult(Problem problem) => StatusCode(
        problem switch
        {
            NotFoundProblem => StatusCodes.Status404NotFound,
            UnauthorizedProblem => StatusCodes.Status401Unauthorized,
            Forbidden => StatusCodes.Status403Forbidden,
            ConflictProblem => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        }
    );
}
