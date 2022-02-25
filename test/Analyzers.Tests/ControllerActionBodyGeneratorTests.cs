using System.Security.Cryptography;
using System.Text;
using Analyzers.Tests.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Xunit.Abstractions;

namespace Analyzers.Tests;

[UsesVerify]
public class ControllerActionBodyGeneratorTests : GeneratorTest
{
    [Fact]
    public async Task Should_Error_If_Controller_Is_Not_Partial()
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
        var source1 = @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public class RocketController : RestfulApiController
{
    [HttpGet]
    public partial Task<ActionResult<IEnumerable<RocketModel>>> ListRockets([FromQuery] ListRockets.Request request);

    [HttpGet(""{id:guid}"")]
    public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);
}
";
        await Verify(await GenerateAsync(source1, source2));
    }

    public ControllerActionBodyGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
    {
        WithGenerator<ControllerActionBodyGenerator>();
        AddReferences(
            typeof(Guid),
            typeof(IRequest),
            typeof(IMediator),
            typeof(Task<>),
            typeof(IEnumerable<>),
            typeof(ControllerBase),
            typeof(Controller),
            typeof(RouteAttribute),
            typeof(RestfulApiController)
        );
        AddSources(
            @"
global using MediatR;
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
"
        );
    }

    [Theory]
    [ClassData(typeof(MethodBodyData))]
    public async Task Should_Generate_Method_Bodies(string[] sources)
    {
        await Verify(await GenerateAsync(sources)).UseParameters(GetSourcesKey(sources.Skip(1)));
    }

    private string GetSourcesKey(IEnumerable<string> sources)
    {
        var a = sources.ToList();
        if (a.Count > 1)
        {
            var hashes = a.Select(x => MD5.HashData(Encoding.Default.GetBytes(x)))
                          .Select(z => string.Join("", z.Select(x => x.ToString("X"))))
                          .ToList();
            sources = new[] { hashes.Aggregate("", (a, b) => a + b) };
        }

        return string.Join(
            "", sources.Select(x => MD5.HashData(Encoding.Default.GetBytes(x)))
                       .Select(z => string.Join("", z.Select(x => x.ToString("X"))))
        );
    }

    private class MethodBodyData : TheoryData<string[]>
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
                    @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpGet(""{id:guid}"")]
    public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);
}"
                }
            );
            Add(
                new[]
                {
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
                    @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpPost(""{id:guid}"")]
    public partial Task<ActionResult> SaveRocket([BindRequired][FromRoute] Guid id, [FromRoute] SaveRocket.Request request);
}"
                }
            );
            Add(
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
        public string Sn { get; init; } = null!;
    }
}",
                    @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpPost(""{id:guid}"")]
    public partial Task<ActionResult<RocketModel>> Save2Rocket([BindRequired][FromRoute] Guid id, [BindRequired] [FromRoute] Save2Rocket.Request request);
}"
                }
            );
            Add(
                new[]
                {
                    defaultString,
                    @"
namespace TestNamespace;
public static class ListRockets
{
    // TODO: Paging model!
    public record Request : IStreamRequest<IEnumerable<RocketModel>>;
}",
                    @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpGet]
    public partial IAsyncEnumerable<RocketModel> ListRockets(ListRockets.Request model);
}"
                }
            );

            Add(
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
                    @"
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpGet(""{id:guid}"")]
    public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);

    [HttpPost]
    [Created(nameof(GetRocket))]
    public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);
}"
                }
            );
            Add(
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
                    @"
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocket.Surgery.LaunchPad.AspNetCore;
using TestNamespace;

namespace MyNamespace.Controllers;

[Route(""[controller]"")]
public partial class RocketController : RestfulApiController
{
    [HttpGet(""{id:guid}"")]
    public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);

    [HttpPost]
    [Accepted(nameof(GetRocket))]
    [ProducesResponseType(202)]
    public partial Task<ActionResult<CreateRocket.Response>> CreateRocket(CreateRocket.Request request);
}"
                }
            );
        }
    }
}
