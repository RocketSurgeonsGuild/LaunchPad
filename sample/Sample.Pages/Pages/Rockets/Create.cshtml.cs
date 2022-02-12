using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketCreateModel : MediatorPageModel
{
    [BindProperty] public CreateRocket.Request Model { get; set; } = new();

    [UsedImplicitly]
    public void OnGet()
    {
    }

    [UsedImplicitly]
    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await Send(Model);
        return RedirectToPage("Index");
    }
}
