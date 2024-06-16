//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System.Threading;
using System.Security.Claims;
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> GetRocket(IMediator mediator, GetRocket.TrackingRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request.Create()with { ClaimsPrincipal = claimsPrincipal }, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
