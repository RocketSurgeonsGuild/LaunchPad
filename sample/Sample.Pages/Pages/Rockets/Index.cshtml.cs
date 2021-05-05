using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Pages.Pages.Rockets
{
    public class RocketIndexModel : PageModel
    {
        private readonly IMediator _mediator;

        // [BindProperty]
        // public Movie Movie { get; set; }
        public IEnumerable<RocketModel> Rockets { get; set; }

        public RocketIndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGet()
        {
            Rockets = await _mediator.Send(new ListRockets.Request());
        }
    }
}