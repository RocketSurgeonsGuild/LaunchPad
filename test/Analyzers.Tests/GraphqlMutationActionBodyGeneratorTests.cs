using System.Security.Claims;
using Analyzers.Tests.Helpers;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.Analyzers;

namespace Analyzers.Tests;

public class GraphqlMutationActionBodyGeneratorTests : GeneratorTest
{
    [Fact]
    public async Task Should_Error_If_Class_Is_Not_Partial()
    {
        var source2 = @"
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
        var source1 = @"
using TestNamespace;

namespace MyNamespace.MyGraph;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class RocketMutation
{
    public partial Task<IEnumerable<RocketModel>> ListRockets([Service] IMediator mediator, ListRockets.Request request);
}
";
        await Verify(await GenerateAsync(source1, source2));
    }

    public GraphqlMutationActionBodyGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
    {
        WithGenerator<GraphqlMutationActionBodyGenerator>();
        AddReferences(
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
        );
        AddSources(
            @"
global using MediatR;
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using HotChocolate;
global using HotChocolate.Types;
"
        );
    }

    [Theory]
    [ClassData(typeof(MethodBodyData))]
    public async Task Should_Generate_Method_Bodies(string key, string[] sources)
    {
        await Verify(await GenerateAsync(sources)).UseParameters(key, "");
    }

    private sealed  class MethodBodyData : TheoryData<string, string[]>
    {
        private const string defaultString = @"
namespace TestNamespace;
public record RocketModel
{
    public Guid Id { get; init; }
    public string Sn { get; init; } = null!;
}
";

        public MethodBodyData()
        {
            Add(
                "GenerateBodyForRequest",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodiesWithMultipleParameters",
                new[]
                {
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
    public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords([Service] IMediator mediator, GetRocketLaunchRecords.Request request);

    /// <summary>
    /// Get a specific launch record for a given rocket
    /// </summary>
    /// <returns></returns>
    public partial Task<LaunchRecordModel> GetRocketLaunchRecord([Service] IMediator mediator, GetRocketLaunchRecord.Request request);
}"
                }
            );
            Add(
                "GenerateBodiesWithMultipleParameters2",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodiesWithMultipleParameters3",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodiesWithMultipleParameters4",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodyWithClaimsPrincipal",
                new[]
                {
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
    public partial Task<RocketModel> Save2Rocket([Service] IMediator mediator, Save2Rocket.Request request, ClaimsPrincipal claimsPrincipal);
}"
                }
            );
            Add(
                "GenerateBodyWithDifferentlyNamedClaimsPrincipal",
                new[]
                {
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
                }
            );
            Add(
                "GenerateBodyWithoutClaimsPrincipal",
                new[]
                {
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
        public ClaimsPrincipal ClaimsPrincipal { get; init; }
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
}"
                }
            );
            Add(
                "GenerateBodyWithCancellationToken",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodyWithDifferentlyNamedCancellationToken",
                new[]
                {
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
}"
                }
            );
            Add(
                "GenerateBodyWithoutCancellationToken",
                new[]
                {
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
}"
                }
            );
        }
    }
}
