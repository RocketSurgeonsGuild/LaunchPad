//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
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
            var _request = request;
            _request.ClaimsPrincipal = cp;
            var result = await mediator.Send(_request).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore
