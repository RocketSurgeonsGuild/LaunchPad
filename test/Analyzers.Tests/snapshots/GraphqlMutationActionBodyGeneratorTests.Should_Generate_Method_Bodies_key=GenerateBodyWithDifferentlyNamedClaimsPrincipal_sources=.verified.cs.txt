//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Methods.cs
#nullable enable
using TestNamespace;
using System.Security.Claims;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> Save2Rocket(IMediator mediator, Save2Rocket.Request request, ClaimsPrincipal cp)
        {
            request.ClaimsPrincipal = cp;
            var result = await Mediator.Send(request).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore
