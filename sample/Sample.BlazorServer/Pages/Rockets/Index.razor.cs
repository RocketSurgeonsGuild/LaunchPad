using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IMediator Mediator { get; set; }

        public IEnumerable<RocketModel> Rockets { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Rockets = await Mediator.Send(new ListRockets.Request() { });
        }
    }
}