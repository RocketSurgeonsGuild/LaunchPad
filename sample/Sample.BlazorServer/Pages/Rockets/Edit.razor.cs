using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Operations.Rockets;

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

        public EditRocket.Model Model { get; set; } = new EditRocket.Model();

        protected override async Task OnInitializedAsync()
        {
            Model = Mapper.Map<EditRocket.Model>(await Mediator.Send(new GetRocket.Request() { Id = Id }));
        }

        public async Task Save()
        {
            await Mediator.Send(EditRocket.CreateRequest(Id, Model, Mapper));
            NavigationManager.NavigateTo("/rockets");
        }
    }
}