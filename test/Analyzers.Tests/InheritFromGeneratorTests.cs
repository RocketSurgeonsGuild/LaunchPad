using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using ZeroAlloc.Mediator;

namespace Analyzers.Tests;

public class InheritFromGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
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
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket+Request must be made partial.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
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
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket must be made partial.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_With_Method_For_Record()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
            public string Excluded { get; set; }
        }

        [InheritFrom(typeof(Model), Exclude = new[] { nameof(Model.Excluded) })]
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
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_With_Method_For_Record_Exclude_Attribute()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
            [ExcludeFromGeneration]
            public string Excluded { get; set; }
            [GenerationIgnore]
            public string Excluded2 { get; set; }
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
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_And_Ignore_Type_Declaration_Members()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }

            private class Mapper;
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
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    public async Task Should_Inherit_Multiple_With_Method_For_Record()
    {
        var result = await Builder
                          .AddReferences(typeof(ImmutableArray<>))
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.Immutable;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
            public ImmutableArray<string> Items { get; set; }
        }
    }
}",
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
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
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

#if !ROSLYN4_0
    [Test]
    public async Task Should_Inherit_Using_Generic_Type_Arguments()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
            public string Excluded { get; set; }
        }

        public partial record Other
        {
            public string OtherNumber { get; set; }
        }

        [InheritFrom<Model>(Exclude = new[] { nameof(Model.Excluded) })]
        [InheritFrom<Other>]
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
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
#endif

    [Test]
    public async Task Should_Generate_With_Method_For_Record_That_Inherits()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
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
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_With_Method_For_Class()
    {
        var result = await Builder
                          .AddSources(
                               @"
using System;
using ZeroAlloc.Mediator;
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
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_With_Method_For_Record_From_Other_Assembly()
    {
        var contracts = await CompileContracts(
            @"
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Contracts
{
    public interface IHaveSerialNumber
    {
        string SerialNumber { get; }
    }

    public enum RocketKind { Falcon, Atlas }

    public record RocketContract : IHaveSerialNumber
    {
        [Description(""The serial number"")]
        [StringLength(30, MinimumLength = 10)]
        public required string SerialNumber { get; set; }
        public string? Nickname { get; init; }
        [DefaultValue(RocketKind.Atlas)]
        public RocketKind Kind { get; set; } = RocketKind.Atlas;
        public int? Stages { get; set; }
        public ImmutableArray<string> Payloads { get; set; }
        public string Excluded { get; set; } = """";
        [ExcludeFromGeneration]
        public string ExcludedByAttribute { get; set; } = """";
    }
}
"
        );
        var result = await Builder
                          .AddReferences(
                               typeof(ImmutableArray<>),
                               typeof(System.ComponentModel.DescriptionAttribute),
                               typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute)
                           )
                          .AddCompilationReferences(contracts)
                          .AddSources(
                               @"
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Contracts;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [InheritFrom(typeof(RocketContract), Exclude = new[] { nameof(RocketContract.Excluded) })]
        public partial record Request : IRequest<RocketModel>
        {
            public Guid Id { get; init; }
        }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_With_Method_For_Class_From_Other_Assembly()
    {
        var contracts = await CompileContracts(
            @"
namespace Sample.Contracts
{
    public class RocketContract
    {
        public string SerialNumber { get; set; } = """";
        public DateTimeOffset? LaunchedAt { get; init; }
        public int Attempts;
        public string? @class { get; set; }
    }
}
"
        );
        var result = await Builder
                          .AddCompilationReferences(contracts)
                          .AddSources(
                               @"
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    [InheritFrom(typeof(Sample.Contracts.RocketContract))]
    public partial class Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

#if !ROSLYN4_0
    [Test]
    public async Task Should_Inherit_From_Same_And_Other_Assembly()
    {
        var contracts = await CompileContracts(
            @"
namespace Sample.Contracts
{
    public record LaunchContract(string Site, DateTimeOffset Window);
}
"
        );
        var result = await Builder
                          .AddCompilationReferences(contracts)
                          .AddSources(
                               @"
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Contracts;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom<Model>]
        [InheritFrom<LaunchContract>(Exclude = new[] { nameof(LaunchContract.Window) })]
        public partial record Request : IRequest<RocketModel>
        {
            public Guid Id { get; init; }
        }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
#endif

    [Test]
    public async Task Should_Report_Members_That_Cannot_Be_Copied_From_Other_Assembly()
    {
        var contracts = await CompileContracts(
            @"
namespace Sample.Contracts
{
    public record RocketContract
    {
        public const int MaximumStages = 3;
        public static RocketContract Empty { get; } = new();
        public readonly int Version;
        public string SerialNumber { get; set; } = """";
        public string Name { get; private set; } = """";
        public string Display => SerialNumber;
        public event EventHandler? Launched;
        public string Describe() => SerialNumber;
        public override string ToString() => SerialNumber;
    }
}
"
        );
        var result = await Builder
                          .AddCompilationReferences(contracts)
                          .WithDiagnosticSeverity(DiagnosticSeverity.Warning)
                          .AddSources(
                               @"
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Contracts;

namespace Sample.Core.Operations.Rockets
{
    [InheritFrom(typeof(RocketContract))]
    public partial record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    private Task<GeneratorTestResults> CompileContracts(string source) =>
        CompileReferencedAssembly(
            "Sample.Contracts",
            [
                typeof(InheritFromAttribute),
                typeof(ImmutableArray<>),
                typeof(System.ComponentModel.DescriptionAttribute),
                typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute),
            ],
            "global using System;\n" + source,
            TestContext.CancellationToken
        );

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<InheritFromGenerator>()
                 .AddReferences(
                      typeof(InheritFromAttribute),
                      typeof(IRequest<>)
                  )
                 .AddSources(
                      @"
global using System;
global using ZeroAlloc.Mediator;
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
