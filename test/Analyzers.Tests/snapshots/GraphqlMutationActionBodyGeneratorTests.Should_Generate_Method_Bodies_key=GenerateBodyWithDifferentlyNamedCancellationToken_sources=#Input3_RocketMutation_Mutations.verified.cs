//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using TestNamespace;
using System.Threading;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> Save2Rocket(IMediator mediator, Save2Rocket.Request request, CancellationToken token)
        {
            var _request = request;
            var result = await mediator.Send(_request, token).ConfigureAwait(false);
            return result;
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
