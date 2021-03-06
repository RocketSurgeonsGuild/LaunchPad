using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Operations.Rockets;
using System;
using System.Threading.Tasks;

namespace Sample.BlazorServer.Pages.Rockets
{
    public partial class Edit : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IMediator Mediator { get; set; }

        [Inject]
        private IMapper Mapper { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        public EditRocket.Request Model { get; set; } = new EditRocket.Request();

        protected override async Task OnInitializedAsync()
        {
            Model = Mapper.Map<EditRocket.Request>(await Mediator.Send(new GetRocket.Request { Id = Id }));
        }

        public async Task Save()
        {
            await Mediator.Send(Model with { Id = Id });
            NavigationManager.NavigateTo("/rockets");
        }
    }
}