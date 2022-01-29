using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Operations.Rockets;

namespace Sample.Pages.Pages.Rockets;

public class RocketCreateModel : MediatorPageModel
{
    private readonly IMapper _mapper;

    public RocketCreateModel(IMapper mapper)
    {
        _mapper = mapper;
    }

    [BindProperty] public CreateRocket.Request Model { get; set; } = new CreateRocket.Request();

    public void OnGet()
    {
    }

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
