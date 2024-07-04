//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlMutationActionBodyGenerator/Input3_RocketMutation_Mutations.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
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
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
