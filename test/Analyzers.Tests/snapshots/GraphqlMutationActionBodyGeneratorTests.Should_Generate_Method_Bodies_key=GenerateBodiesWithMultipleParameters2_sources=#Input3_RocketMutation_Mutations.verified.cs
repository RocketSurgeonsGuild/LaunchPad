//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        /// <summary>
        /// Get a specific launch record for a given rocket
        /// </summary>
        /// <returns></returns>
        public partial async Task<LaunchRecordModel> GetRocketLaunchRecord(IMediator mediator, GetRocketLaunchRecord.Request request)
        {
            var result = await mediator.Send(request).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore
