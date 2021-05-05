using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IMediator Mediator { get; set; }

        public IEnumerable<RocketModel> Rockets { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Rockets = await Mediator.Send(new ListRockets.Request());
        }
    }
}