using Microsoft.AspNetCore.Components;

using Sample.BlazorWasm.Forms;

namespace Sample.BlazorWasm.Pages.Rockets;

public partial class Edit : ComponentBase
{
    [Parameter]
    public Guid Id { get; set; }

    public RocketForm Model { get; set; } = new();

    public ProblemDetails? Problem { get; private set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IRocketsClient Client { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Model = RocketForm.From(RocketRequest.From(await Client.GetRocket(Id)));
        }
        catch (Exception exception) when (exception.IsFailedCall())
        {
            Problem = exception.ToProblem();
        }
    }

    public async Task Save()
    {
        try
        {
            await Client.EditRocket(Id, Model.ToImmutable().ToEditRequest(Id));
        }
        catch (Exception exception) when (exception.IsFailedCall())
        {
            Problem = exception.ToProblem();
            return;
        }

        NavigationManager.NavigateTo("rockets");
    }
}
