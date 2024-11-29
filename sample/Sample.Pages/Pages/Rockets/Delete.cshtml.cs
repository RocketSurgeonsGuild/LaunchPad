using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketDeleteModel : RocketViewModel
{
    public async Task<ActionResult> OnPost()
    {
        await Send(new DeleteRocket.Request(Id));
        return RedirectToPage("Index");
    }
}
