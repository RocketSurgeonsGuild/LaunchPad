using Analyzers.Tests.Helpers;
using MediatR;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class InheritFromGeneratorTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();
        result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket+Request must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();
        result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_With_Method_For_Record()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Fact]
    public async Task Should_Inherit_Multiple_With_Method_For_Record()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        public partial record Other
        {
            public string OtherNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        [InheritFrom(typeof(Other))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_With_Method_For_Record_That_Inherits()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : Model, IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_With_Method_For_Class()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial class Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial class Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<InheritFromGenerator>()
                 .AddReferences(
                      typeof(InheritFromAttribute),
                      typeof(IMediator),
                      typeof(IBaseRequest)
                  )
                 .AddSources(
                      @"
global using System;
global using MediatR;
global using Sample.Core.Operations.Rockets;
namespace Sample.Core.Operations.Rockets
{
    public class RocketModel
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
    }
}
"
                  );
    }
}