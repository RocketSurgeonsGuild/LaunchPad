//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
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
            var result = mediator.CreateStream(request);
            return result;
        }

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
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
