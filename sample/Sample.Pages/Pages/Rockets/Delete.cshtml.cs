using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketDeleteModel : RocketViewModel
{
    public async Task<IActionResult> OnPost() =>
        ( await Mediator.Send(new DeleteRocket.Request(Id), HttpContext.RequestAborted) ).Value.TryGetValue<ZeroAlloc.Mediator.Unit>(out _, out var problem)
            ? RedirectToPage("Index")
            : ProblemResult(problem);
}
