//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/RocketMutation_Methods.cs
#nullable enable
using TestNamespace;
using MediatR;

namespace MyNamespace.Controllers
{
    public partial class RocketMutation
    {
        /// <summary>
        /// Get the launch records for a given rocket
        /// </summary>
        /// <returns></returns>
        public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords([HotChocolate.ServiceAttribute] IMediator mediator, GetRocketLaunchRecords.Request request)
        {
            var result = Mediator.CreateStream(request);
            return result;
        }

        /// <summary>
        /// Get a specific launch record for a given rocket
        /// </summary>
        /// <returns></returns>
        public partial async Task<LaunchRecordModel> GetRocketLaunchRecord(IMediator mediator, GetRocketLaunchRecord.Request request)
        {
            var result = await Mediator.Send(request).ConfigureAwait(false);
            return result;
        }
    }
}
#nullable restore
