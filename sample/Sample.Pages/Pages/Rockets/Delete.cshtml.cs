using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations.Rockets;
using System.Threading.Tasks;

namespace Sample.Pages.Pages.Rockets
{
    public class RocketDeleteModel : RocketViewModel
    {
        public async Task<ActionResult> OnPost()
        {
            await Send(new DeleteRocket.Request { Id = Id });
            return RedirectToPage("Index");
        }
    }
}