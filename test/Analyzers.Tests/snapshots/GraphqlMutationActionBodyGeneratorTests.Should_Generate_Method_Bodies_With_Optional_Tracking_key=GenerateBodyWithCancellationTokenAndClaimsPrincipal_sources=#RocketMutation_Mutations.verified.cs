﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Mutations.cs
#nullable enable
using TestNamespace;
using System.Threading;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> Save2Rocket(IMediator mediator, Save2Rocket.TrackingRequest request, ClaimsPrincipal cp, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request.Create(), cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore