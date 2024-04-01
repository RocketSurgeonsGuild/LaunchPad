//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Mutations.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<Unit> GetRocket(IMediator mediator, GetRocket.Request request)
        {
            await mediator.Send(request).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
#nullable restore
