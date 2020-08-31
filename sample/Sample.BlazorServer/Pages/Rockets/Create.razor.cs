using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class Create : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IMediator Mediator { get; set; }

        public CreateRocket.Request Model { get; set; } = new CreateRocket.Request();

        public async Task Save()
        {
            await Mediator.Send(Model);
            NavigationManager.NavigateTo("/rockets");
        }
    }
}