using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
// PageModel has a NotFound() method, so the problem case needs an alias
using NotFoundProblem = Rocket.Surgery.LaunchPad.Foundation.NotFound;

namespace Sample.Pages.Pages.Rockets;

public class RocketEditModel : RocketViewModel
{
    [BindProperty]
    public EditRocket.Request Model { get; set; } = new();

    public override async Task<IActionResult> OnGet()
    {
        var result = await base.OnGet();
        if (result is PageResult) Model = EditRocket.MapRequest(Rocket);
        return result;
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return await base.OnGet();

        if (!( await Mediator.Send(Model with { Id = Id, }, HttpContext.RequestAborted) ).Value.TryGetValue<Core.Models.RocketModel>(out _, out var problem))
        {
            if (problem is NotFoundProblem) return NotFound();
            ModelState.AddModelError(string.Empty, problem.Message());
            return await base.OnGet();
        }

        return RedirectToPage("Index");
    }
}
