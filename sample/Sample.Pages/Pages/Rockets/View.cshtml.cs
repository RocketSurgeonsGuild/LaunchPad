using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rocket.Surgery.LaunchPad.Pages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets
{
    public class RocketViewModel : MediatorPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public RocketModel Rocket { get; set; }

        public virtual async Task OnGet()
        {
            Rocket =  await Send(new GetRocket.Request() { Id = Id });
        }
    }
}