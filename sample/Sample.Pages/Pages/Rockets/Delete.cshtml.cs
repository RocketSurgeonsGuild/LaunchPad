using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rocket.Surgery.LaunchPad.Pages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets
{
    public class RocketDeleteModel : RocketViewModel
    {
        public async Task<ActionResult> OnPost()
        {
            await Send(new DeleteRocket.Request() { Id = Id });
            return RedirectToPage("Index");
        }
    }
}