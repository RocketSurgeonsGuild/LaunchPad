//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Mutations.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<Unit> GetRocket(IMediator mediator, GetRocket.TrackingRequest request)
        {
            await mediator.Send(request.Create()).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
#nullable restore
