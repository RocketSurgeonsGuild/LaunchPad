using System.Security.Claims;

using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Analyzers.Tests;

public class ControllerActionBodyGeneratorTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
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
}
public enum RocketType { A, B }
",
                               @"
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; init; } = null!;
    }
}",
                               """
                               using Microsoft.AspNetCore.Mvc;
                               using Microsoft.AspNetCore.Mvc.ModelBinding;
                               using Rocket.Surgery.LaunchPad.AspNetCore;
                               using TestNamespace;

                               namespace MyNamespace.Controllers;

                               [Route("[controller]")]
                               public partial class RocketController : RestfulApiController
                               {
                                   [HttpPost("{id:guid}/{sn?}")]
                                   public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [FromRoute] string? sn, [BindRequired] [FromRoute] Save2Rocket.Request request);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Fact]
    public async Task Should_Error_If_Controller_Is_Not_Partial()
    {
        var result = await Builder
                          .AddSources(
                               @"
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

",
                               """
                               using Microsoft.AspNetCore.Mvc;
                               using Microsoft.AspNetCore.Mvc.ModelBinding;
                               using Rocket.Surgery.LaunchPad.AspNetCore;
                               using TestNamespace;

                               namespace MyNamespace.Controllers;

                               [Route("[controller]")]
                               public class RocketController : RestfulApiController
                               {
                                   [HttpGet]
                                   public partial Task<ActionResult<IEnumerable<RocketModel>>> ListRockets([FromQuery] ListRockets.Request request);
                               
                                   [HttpGet("{id:guid}")]
                                   public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Theory]
    [ClassData(typeof(MethodBodyData))]
    public async Task Should_Generate_Method_Bodies(string key, string[] sources) => await Verify(Builder.AddSources(sources).Build().GenerateAsync()).UseParameters(key, "");

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
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpGet("{id:guid}")]
                        public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodyWithIdParameterAndAddBindRequired",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class SaveRocket
{
    public record Request : IRequest
    {
        public Guid Id { get; set; }
    }
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpPost("{id:guid}")]
                        public partial Task<ActionResult> SaveRocket([BindRequired][FromRoute] Guid id, [FromRoute] SaveRocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodyWithIdParameter",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string Sn { get; init; } = null!;
    }
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpPost("{id:guid}")]
                        public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [BindRequired] [FromRoute] Save2Rocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodyWithIdParameterMultiple",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class Save2Rocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; set; } = null!;
    }
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpPost("{id:guid}/{sn?}")]
                        public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [FromRoute] string? sn, [BindRequired] [FromRoute] Save2Rocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodyForListAction",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class ListRockets
{
    // TODO: Paging model!
    public record Request : IStreamRequest<RocketModel>;
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpGet]
                        public partial IAsyncEnumerable<RocketModel> ListRockets(ListRockets.Request model);
                    }
                    """,
                ]
            );

            Add(
                "GenerateBodiesWithCreatedReturn",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class GetRocket
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
    }
}

public static class CreateRocket
{
    public record Request : IRequest<Response>
    {
        public string SerialNumber { get; set; } = null!;
        public RocketType Type { get; set; }
    }

    public record Response
    {
        public Guid Id { get; init; }
    }
}",
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpGet("{id:guid}")]
                        public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] GetRocket.Request request);
                    
                        [HttpPost]
                        [Created(nameof(GetRocket))]
                        public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodiesWithAcceptReturnType",
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
}

public static class CreateRocket
{
    public record Request : IRequest<Response>
    {
        public string SerialNumber { get; set; } = null!;
        public RocketType Type { get; set; }
    }

    public record Response
    {
        public Guid Id { get; init; }
    }
}",
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpGet("{id:guid}")]
                        public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] GetRocket.Request request);
                    
                        [HttpPost]
                        [Accepted(nameof(GetRocket))]
                        [ProducesResponseType(202)]
                        public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodiesWithVoidReturnType",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class DeleteRocket
{
    public record Request : IRequest
    {
        public Guid Id { get; set; }
    }
}",
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpGet("{id:guid}")]
                        public partial Task<ActionResult> DeleteRocket([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] DeleteRocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodiesWithVoidReturnTypeOther",
                [
                    defaultString,
                    @"
namespace TestNamespace;
public static class DeleteLaunchRecord
{
    public record Request : IRequest
    {
        public Guid Id { get; set; }
    }
}",
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpDelete("{id:guid}")]
                        public partial Task<ActionResult> DeleteLaunchRecord([BindRequired] [FromRoute] Guid id, DeleteLaunchRecord.Request request);
                    }
                    """,
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
        public Guid LaunchRecordId { get; init; }
    }
}",
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        /// <summary>
                        /// Get the launch records for a given rocket
                        /// </summary>
                        /// <returns></returns>
                        [HttpGet("{id:guid}/launch-records")]
                        public partial IAsyncEnumerable<LaunchRecordModel> GetRocketLaunchRecords([BindRequired] [FromRoute] Guid id, GetRocketLaunchRecords.Request request);
                    
                        /// <summary>
                        /// Get a specific launch record for a given rocket
                        /// </summary>
                        /// <returns></returns>
                        [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
                        public partial Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] Guid launchRecordId, GetRocketLaunchRecord.Request request);
                    }
                    """,
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
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        /// <summary>
                        /// Get a specific launch record for a given rocket
                        /// </summary>
                        /// <returns></returns>
                        [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
                        public partial Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] Guid launchId, GetRocketLaunchRecord.Request request);
                    }
                    """,
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
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        /// <summary>
                        /// Get a specific launch record for a given rocket
                        /// </summary>
                        /// <returns></returns>
                        [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
                        public partial Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] Guid launchRecordId, GetRocketLaunchRecord.Request request);
                    }
                    """,
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
                    """

                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;
                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        /// <summary>
                        /// Get a specific launch record for a given rocket
                        /// </summary>
                        /// <returns></returns>
                        [HttpGet("{id:guid}/launch-records/{launchRecordId:guid}")]
                        public partial Task<ActionResult<LaunchRecordModel>> GetRocketLaunchRecord([BindRequired] [FromRoute] Guid id, [BindRequired] [FromRoute] string launchRecordId, GetRocketLaunchRecord.Request request);
                    }
                    """,
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
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; set; }
        public string? Sn { get; set; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string Other { get; set; }
    }
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;

                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpPost("{id:guid}/{sn?}")]
                        public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [FromRoute] string? sn, [BindRequired] [FromRoute] Save2Rocket.Request request);
                    }
                    """,
                ]
            );
            Add(
                "GenerateBodyWithHttpRequest",
                [
                    defaultString,
                    @"
using Microsoft.AspNetCore.Http;
namespace TestNamespace;
public static class Save2Rocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string? Sn { get; init; } = null!;
        public HttpRequest R { get; init; }
        public string Other { get; init; }
    }
}",
                    """
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Mvc.ModelBinding;
                    using Rocket.Surgery.LaunchPad.AspNetCore;

                    using TestNamespace;

                    namespace MyNamespace.Controllers;

                    [Route("[controller]")]
                    public partial class RocketController : RestfulApiController
                    {
                        [HttpPost("{id:guid}/{sn?}")]
                        public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [FromRoute] string? sn, [BindRequired] [FromRoute] Save2Rocket.Request request);
                    }
                    """,
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
public enum RocketType { A, B }
";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<ControllerActionBodyGenerator>()
                 .AddReferences(
                      typeof(Guid),
                      typeof(IRequest),
                      typeof(IMediator),
                      typeof(Task<>),
                      typeof(IEnumerable<>),
                      typeof(ControllerBase),
                      typeof(Controller),
                      typeof(RouteAttribute),
                      typeof(RestfulApiController),
                      typeof(ClaimsPrincipal),
                      typeof(HttpContext),
                      typeof(ProblemDetails),
                      typeof(IValidator),
                      typeof(IValidatorInterceptor)
                  )
                 .AddSources(
                      @"
global using MediatR;
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
"
                  );
    }
}
