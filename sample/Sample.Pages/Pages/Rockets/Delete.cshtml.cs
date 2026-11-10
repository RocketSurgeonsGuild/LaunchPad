using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketDeleteModel : RocketViewModel
{
    public async Task<ActionResult> OnPost()
    {
        await Mediator.Send(new DeleteRocket.Request(Id), HttpContext.RequestAborted);
        return RedirectToPage("Index");
    }
}
