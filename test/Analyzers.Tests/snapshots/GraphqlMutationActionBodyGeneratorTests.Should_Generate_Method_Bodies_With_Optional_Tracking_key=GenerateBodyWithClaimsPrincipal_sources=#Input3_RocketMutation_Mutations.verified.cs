//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using TestNamespace;
using System.Security.Claims;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> Save2Rocket(IMediator mediator, ClaimsPrincipal claimsPrincipal, Save2Rocket.TrackingRequest request)
        {
            var result = await mediator.Send(request.Create()).ConfigureAwait(false);
            return result;
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
