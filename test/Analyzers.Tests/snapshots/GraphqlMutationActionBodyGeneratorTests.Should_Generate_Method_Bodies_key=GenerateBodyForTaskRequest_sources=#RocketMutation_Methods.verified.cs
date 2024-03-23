//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Methods.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task GetRocket(IMediator mediator, GetRocket.Request request)
        {
            await mediator.Send(request).ConfigureAwait(false);
        }
    }
}
#nullable restore
