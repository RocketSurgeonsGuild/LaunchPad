using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Operations.Rockets;
using System.Threading.Tasks;

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