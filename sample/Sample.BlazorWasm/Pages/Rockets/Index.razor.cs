using Microsoft.AspNetCore.Components;

namespace Sample.BlazorWasm.Pages.Rockets;

public partial class Index : ComponentBase
{
    public IEnumerable<RocketModel> Rockets { get; set; } = [];

    public ProblemDetails? Problem { get; private set; }

    [Inject] private IRocketsClient Client { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Rockets = await Client.ListRockets(null).ToListAsync();
        }
        catch (Exception exception) when (exception.IsFailedCall())
        {
            Problem = exception.ToProblem();
        }
    }
}
