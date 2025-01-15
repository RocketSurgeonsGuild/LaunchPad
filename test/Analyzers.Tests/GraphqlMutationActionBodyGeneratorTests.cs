using System.Security.Claims;

using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;

using MediatR;

using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate;

namespace Analyzers.Tests;

public class GraphqlMutationActionBodyGeneratorTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Error_If_Class_Property_Is_Init()
    {
        var result = await Builder
                          .AddSources(
                               @"
namespace TestNamespace;
public record RocketModel
{
    public Guid Id { get; init; }
    public string Sn { get; init; } = null!;
}",
                               @"
using System.Security.Claims;
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
        public string Other { get; init; }
    }
}",
                               @"
using TestNamespace;
using System.Security.Claims;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, ClaimsPrincipal cp);
}"
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Fact]
    public async Task Should_Error_If_Class_Is_Not_Partial()
    {
        const string source2 = @"
namespace TestNamespace;

public record RocketModel
{
    public Guid Id { get; init; }
    public string Sn { get; init; } = null!;
}

public static class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
}

public static class ListRockets
{
    // TODO: Paging model!
    public record Request : IRequest<IEnumerable<RocketModel>>;
}

";
        const string source1 = @"
using TestNamespace;

namespace MyNamespace.MyGraph;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class RocketMutation
{
    public partial Task<IEnumerable<RocketModel>> ListRockets([Service] IMediator mediator, ListRockets.Request request);
}
";
        await Verify(await Builder.AddSources(source1, source2).Build().GenerateAsync());
    }

    [Theory]
    [ClassData(typeof(MethodBodyData))]
    public async Task Should_Generate_Method_Bodies(string key, string[] sources) => await Verify(await Builder.AddSources(sources).Build().GenerateAsync()).UseParameters(key, "");

    [Theory]
    [ClassData(typeof(MethodBodyWithOptionalTrackingData))]
    public async Task Should_Generate_Method_Bodies_With_Optional_Tracking(string key, string[] sources) => await Verify(
            await Builder
                 .WithGenerator<GraphqlOptionalPropertyTrackingGenerator>()
                 .WithGenerator<PropertyTrackingGenerator>()
                 .AddReferences(typeof(IOptionalTracking<>), typeof(IPropertyTracking))
                 .AddSources(sources)
                 .Build()
                 .GenerateAsync()
        )
       .UseParameters(key, "");

    private sealed class MethodBodyData : TheoryData<string, string[]>
    {
        public MethodBodyData()
        {
            Add(
                "GenerateBodyForRequest",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.Request request);
}",
                ]
            );
            Add(
                "GenerateBodyForTaskRequest",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class GetRocket
{
    public record Request : IRequest
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<Unit> GetRocket([Service] IMediator mediator, GetRocket.Request request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static class GetRocketLaunchRecords
{
    public record Request : IStreamRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }
    }
}

public static class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get the launch records for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords(IMediator mediator, GetRocketLaunchRecords.Request request);

    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.Request request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters2",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public Guid LaunchId { get; init; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.Request request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters3",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public Guid LaunchRecordId { get; init; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.Request request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters4",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public string LaunchRecordId { get; init; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.Request request);
}",
                ]
            );
            Add(
                "GenerateBodyWithClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Security.Claims;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, ClaimsPrincipal claimsPrincipal, Save2Rocket.Request request);
}",
                ]
            );
            Add(
                "GenerateBodyWithDifferentlyNamedClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Security.Claims;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, ClaimsPrincipal cp);
}",
                ]
            );
            Add(
                "GenerateBodyWithoutClaimsPrincipal",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string Other { get; init; }
    }
}",
                    @"

using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request);
}",
                ]
            );
            Add(
                "GenerateBodyWithCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyWithCancellationTokenAndClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, ClaimsPrincipal cp, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyWithDifferentlyNamedCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, CancellationToken token);
}",
                ]
            );
            Add(
                "GenerateBodyWithoutCancellationToken",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request);
}",
                ]
            );
        }

        private const string defaultString = @"
namespace TestNamespace;
public record RocketModel
{
    public Guid Id { get; init; }
    public string Sn { get; init; } = null!;
}
";
    }

    private sealed class MethodBodyWithOptionalTrackingData : TheoryData<string, string[]>
    {
        public MethodBodyWithOptionalTrackingData()
        {
            Add(
                "GenerateBodyForRequest",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodyForRequestWithClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public required Guid Id { get; set; }
        public required ClaimsPrincipal ClaimsPrincipal { get; init; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using System.Security.Claims;
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request, ClaimsPrincipal claimsPrincipal);
}",
                ]
            );
            Add(
                "GenerateBodyForRequestWithClaimsPrincipal2",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest(Guid Id, ClaimsPrincipal ClaimsPrincipal) : IPropertyTracking<Request>, IRequest<RocketModel>;
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using System.Security.Claims;
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request, ClaimsPrincipal claimsPrincipal);
}",
                ]
            );
            Add(
                "GenerateBodyForRequestWithCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public required Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using System.Threading;
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyForRequestWithClaimsPrincipalAndCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
using System.Security.Claims;
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public required Guid Id { get; set; }
        public required ClaimsPrincipal ClaimsPrincipal { get; init; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using System.Threading;
using System.Security.Claims;
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyForRequestWithClaimsPrincipalConstructor",
                [
                    defaultString,
                    @"
using System.Threading;
using System.Security.Claims;
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
    public partial record PatchRequest(Guid Id, ClaimsPrincipal ClaimsPrincipal) : IPropertyTracking<Request>, IRequest<RocketModel>;
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using System.Threading;
using System.Security.Claims;
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyForTaskRequest",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static partial class GetRocket
{
    public record Request : IRequest
    {
        public Guid Id { get; set; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>;
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<Unit> GetRocket([Service] IMediator mediator, GetRocket.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static partial class GetRocketLaunchRecords
{
    public record Request : IStreamRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IStreamRequest<LaunchRecordModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}

public static partial class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<LaunchRecordModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get the launch records for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords(IMediator mediator, GetRocketLaunchRecords.TrackingRequest request);

    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters2",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static partial class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public Guid LaunchId { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<LaunchRecordModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters3",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static partial class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public Guid LaunchRecordId { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<LaunchRecordModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodiesWithMultipleParameters4",
                [
                    defaultString,
                    @"
namespace TestNamespace;

public record LaunchRecordModel
{
    public Guid Id { get; init; }
    public string Partner { get; init; } = null!;
    public string Payload { get; init; } = null!;
}

public static partial class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }

        public string LaunchRecordId { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<LaunchRecordModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodyWithClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Security.Claims;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, ClaimsPrincipal claimsPrincipal, Save2Rocket.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodyWithDifferentlyNamedClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Security.Claims;
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Security.Claims;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request, ClaimsPrincipal cp);
}",
                ]
            );
            Add(
                "GenerateBodyWithoutClaimsPrincipal",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"

using TestNamespace;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request);
}",
                ]
            );
            Add(
                "GenerateBodyWithCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyWithCancellationTokenAndClaimsPrincipal",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request, ClaimsPrincipal cp, CancellationToken cancellationToken);
}",
                ]
            );
            Add(
                "GenerateBodyWithDifferentlyNamedCancellationToken",
                [
                    defaultString,
                    @"
using System.Threading;
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request, CancellationToken token);
}",
                ]
            );
            Add(
                "GenerateBodyWithoutCancellationToken",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static partial class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
        public string Other { get; init; }
    }
    public partial record PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
    public partial record TrackingRequest : IOptionalTracking<PatchRequest>
    {
        public Guid Id { get; set; }
    }
}",
                    @"
using TestNamespace;
using System.Threading;

namespace MyNamespace.Controllers;

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.TrackingRequest request);
}",
                ]
            );
        }

        private const string defaultString = @"
global using Rocket.Surgery.LaunchPad.Foundation;
global using Rocket.Surgery.LaunchPad.HotChocolate;
namespace TestNamespace;
public record RocketModel
{
    public Guid Id { get; init; }
    public string Sn { get; init; } = null!;
}
";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<GraphqlMutationActionBodyGenerator>()
                 .AddReferences(
                      typeof(Guid),
                      typeof(IRequest),
                      typeof(IMediator),
                      typeof(Task<>),
                      typeof(IEnumerable<>),
                      typeof(ExtendObjectTypeAttribute),
                      typeof(ServiceAttribute),
                      typeof(OperationType),
                      typeof(OperationTypeNames),
                      typeof(ClaimsPrincipal)
                  )
                 .AddSources(
                      @"
global using MediatR;
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using HotChocolate;
global using HotChocolate.Types;
global using System.Security.Claims;
"
                  );
    }
}
