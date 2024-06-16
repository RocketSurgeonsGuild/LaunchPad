﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
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
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
