using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class Delete : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IMediator Mediator { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        public RocketModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model =  await Mediator.Send(new GetRocket.Request() { Id = Id });
        }

        public async Task Save()
        {
            await Mediator.Send(new DeleteRocket.Request() { Id = Id });
            NavigationManager.NavigateTo("/rockets");
        }
    }
}