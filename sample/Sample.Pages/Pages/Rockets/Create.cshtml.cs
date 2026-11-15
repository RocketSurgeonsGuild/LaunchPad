using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Operations;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.Pages.Pages.Rockets;

public class RocketCreateModel : PageModel
{
    private IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    [BindProperty]
    public CreateRocket.Request Model { get; set; } = new();

    [UsedImplicitly]
    public void OnGet() { }

    [UsedImplicitly]
    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        if (!( await Mediator.Send(Model, HttpContext.RequestAborted) ).Value.TryGetValue<CreateRocket.Response>(out _, out var problem))
        {
            ModelState.AddModelError(string.Empty, problem.Message());
            return Page();
        }

        return RedirectToPage("Index");
    }
}
