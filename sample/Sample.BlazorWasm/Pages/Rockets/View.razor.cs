using Microsoft.AspNetCore.Components;

namespace Sample.BlazorWasm.Pages.Rockets;

public partial class View : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    public RocketModel? Model { get; set; }

    public ProblemDetails? Problem { get; private set; }

    [Inject] private IRocketsClient Client { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Model = await Client.GetRocket(Id);
        }
        catch (Exception exception) when (exception.IsFailedCall())
        {
            Problem = exception.ToProblem();
        }
    }
}
