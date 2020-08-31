using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class View : ComponentBase
    {
        [Inject] private IMediator Mediator{ get; set; }

        [Parameter]
        public Guid Id { get; set; }

        public RocketModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model =  await Mediator.Send(new GetRocket.Request() { Id = Id });
        }
    }
}