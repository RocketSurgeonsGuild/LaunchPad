using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rocket.Surgery.LaunchPad.Pages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets
{
    public class RocketEditModel : RocketViewModel
    {
        private readonly IMapper _mapper;

        public RocketEditModel(IMapper mapper)
        {
            _mapper = mapper;
        }

        [BindProperty]
        public EditRocket.Model Model { get; set; } = new EditRocket.Model();

        public override async Task OnGet()
        {
            await base.OnGet();
            Model = _mapper.Map<EditRocket.Model>(Rocket);
        }

        public async Task<ActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await base.OnGet();
                return Page();
            }

            await Send(EditRocket.CreateRequest(Id, Model, _mapper));
            return RedirectToPage("Index");
        }
    }
}