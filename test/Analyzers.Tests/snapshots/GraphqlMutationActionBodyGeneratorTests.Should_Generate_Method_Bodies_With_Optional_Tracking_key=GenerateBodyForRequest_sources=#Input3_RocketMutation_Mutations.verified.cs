//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> GetRocket(IMediator mediator, GetRocket.TrackingRequest request)
        {
            var result = await mediator.Send(request.Create()).ConfigureAwait(false);
            return result;
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
