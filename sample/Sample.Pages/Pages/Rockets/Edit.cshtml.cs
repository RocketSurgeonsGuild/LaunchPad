using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketEditModel : RocketViewModel
{
    [BindProperty] public EditRocket.Request Model { get; set; } = new();

    public override async Task OnGet()
    {
        await base.OnGet();
        Model = EditRocket.MapRequest(Rocket);
    }

    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await base.OnGet();
            return Page();
        }

        await Send(Model with { Id = Id });
        return RedirectToPage("Index");
    }
}
