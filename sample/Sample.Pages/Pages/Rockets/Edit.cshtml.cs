using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Core.Operations.Rockets;
using System.Threading.Tasks;

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
        public EditRocket.Request Model { get; set; } = new();

        public override async Task OnGet()
        {
            await base.OnGet();
            Model = _mapper.Map<EditRocket.Request>(Rocket);
        }

        public async Task<ActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await base.OnGet();
                return Page();
            }

            await Send(Model with {Id = Id});
            return RedirectToPage("Index");
        }
    }
}