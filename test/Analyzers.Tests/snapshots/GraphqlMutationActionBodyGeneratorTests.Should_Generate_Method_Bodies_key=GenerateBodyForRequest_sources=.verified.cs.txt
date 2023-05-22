//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Methods.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        public partial async Task<RocketModel> GetRocket(IMediator mediator, GetRocket.Request request)
        {
            var result = await Mediator.Send(request).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore
