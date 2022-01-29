using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Core.Operations.Rockets;

namespace Sample.BlazorServer.Pages.Rockets;

public partial class Edit : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    public EditRocket.Request Model { get; set; } = new EditRocket.Request();

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IMediator Mediator { get; set; }

    [Inject] private IMapper Mapper { get; set; }

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
