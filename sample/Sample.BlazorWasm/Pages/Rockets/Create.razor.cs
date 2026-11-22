using Microsoft.AspNetCore.Components;

using Sample.BlazorWasm.Forms;

namespace Sample.BlazorWasm.Pages.Rockets;

public partial class Create : ComponentBase
{
    public RocketForm Model { get; set; } = new();

    public ProblemDetails? Problem { get; private set; }

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private IRocketsClient Client { get; set; } = null!;

    public async Task Save()
    {
        try
        {
            await Client.CreateRocket(Model.ToImmutable().ToCreateRequest());
        }
        catch (Exception exception) when (exception.IsFailedCall())
        {
            Problem = exception.ToProblem();
            return;
        }

        NavigationManager.NavigateTo("rockets");
    }
}
