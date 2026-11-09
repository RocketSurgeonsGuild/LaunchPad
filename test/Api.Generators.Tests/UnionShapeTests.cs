using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.LaunchPad.Api.Generators;

namespace Api.Generators.Tests;

/// <summary>
///     The union detection contract that generators share (docs/design/union-types.md, section 4): every way of declaring a union, in source and in
///     metadata, is read the same way.
/// </summary>
public class UnionShapeTests
{
    // What a .NET 10 project gets from LaunchPad's union generator; .NET 11 has these in the BCL.
    private const string Polyfill = """
                                    namespace System.Runtime.CompilerServices
                                    {
                                        [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
                                        internal sealed class UnionAttribute : System.Attribute;
                                        internal interface IUnion { object? Value { get; } }
                                    }
                                    """;

    // Stand-ins for LaunchPad.Primitives' standard cases; the contract matches them by full name.
    private const string Primitives = """
                                      namespace Rocket.Surgery.LaunchPad.Primitives
                                      {
                                          public abstract record Problem;
                                          public sealed record NotFound : Problem;
                                          public sealed record Conflict : Problem;
                                      }
                                      """;

    private const string Model = "public record RocketModel(string Id);";

    [Test]
    public async Task Should_Read_A_Native_Union_Declaration()
    {
        var shape = Read(
            """
            using Rocket.Surgery.LaunchPad.Primitives;
            public union GetRocketResult(RocketModel, NotFound, string?);
            """,
            "GetRocketResult"
        );

        await Verify(shape);
    }

    [Test]
    public async Task Should_Read_A_Portable_Union_Before_It_Is_Generated()
    {
        // Only the partial constructor definitions: what another generator in the same compilation sees.
        var shape = Read(
            """
            using System.Runtime.CompilerServices;
            using Rocket.Surgery.LaunchPad.Primitives;

            [Union]
            public readonly partial struct GetRocketResult
            {
                public partial GetRocketResult(RocketModel value);
                public partial GetRocketResult(Problem value);
                public partial GetRocketResult(int? code);
            }
            """,
            "GetRocketResult"
        );

        await Verify(shape);
    }

    [Test]
    public async Task Should_Read_A_Hand_Written_Union_And_Ignore_Other_Constructors()
    {
        var shape = Read(
            """
            using System.Runtime.CompilerServices;
            using Rocket.Surgery.LaunchPad.Primitives;

            [Union]
            public readonly struct GetRocketResult : IUnion
            {
                private readonly object? _value;
                public GetRocketResult(RocketModel value) => _value = value;
                public GetRocketResult(Conflict value) => _value = value;
                internal GetRocketResult(string hidden) => _value = hidden;
                public GetRocketResult(RocketModel value, int ignored) => _value = value;
                public object? Value => _value;
            }
            """,
            "GetRocketResult"
        );

        await Verify(shape);
    }

    [Test]
    public async Task Should_Read_A_Union_From_Metadata_With_Its_Own_Polyfill()
    {
        var library = Compile(
            "Library",
            """
            using Rocket.Surgery.LaunchPad.Primitives;
            public union GetRocketResult(RocketModel, NotFound);
            """,
            includeModel: true
        );
        var consumer = Compile("Consumer", "public class Consumer;", includeModel: false, references: [library.ToMetadataReference()]);
        var type = consumer.GetTypeByMetadataName("GetRocketResult")!;

        UnionShape.TryCreate(type, out var shape).ShouldBeTrue();
        await Verify(shape);
    }

    [Test]
    public async Task Should_Unwrap_Task_And_ValueTask_Responses()
    {
        var compilation = Compile(
            "Server",
            """
            using System.Threading.Tasks;
            using Rocket.Surgery.LaunchPad.Primitives;
            public union GetRocketResult(RocketModel, NotFound);
            public class Handlers
            {
                public Task<GetRocketResult> A() => null!;
                public ValueTask<GetRocketResult> B() => default;
                public GetRocketResult C() => default;
                public Task<RocketModel> D() => null!;
            }
            """,
            includeModel: true
        );
        var handlers = compilation.GetTypeByMetadataName("Handlers")!;
        var found = handlers
                   .GetMembers()
                   .OfType<IMethodSymbol>()
                   .Where(method => method.MethodKind == MethodKind.Ordinary)
                   .Select(method => (method.Name, IsUnion: UnionShape.TryCreateFromResponse(method.ReturnType, out _)));

        await Verify(found);
    }

    [Test]
    public void Should_Not_Treat_Other_Types_As_Unions()
    {
        var compilation = Compile("Plain", "public readonly struct NotAUnion { public NotAUnion(int value) { } }", includeModel: false);

        UnionShape.TryCreate(compilation.GetTypeByMetadataName("NotAUnion")!, out var shape).ShouldBeFalse();
        shape.ShouldBeNull();
    }

    private static UnionShape Read(string source, string typeName)
    {
        var compilation = Compile("Contracts", source, includeModel: true);
        UnionShape.TryCreate(compilation.GetTypeByMetadataName(typeName)!, out var shape).ShouldBeTrue();
        return shape!;
    }

    private static CSharpCompilation Compile(string name, string source, bool includeModel, IEnumerable<MetadataReference>? references = null)
    {
        var options = new CSharpParseOptions(LanguageVersion.Preview);
        var sources = new List<string> { source, Polyfill, };
        if (includeModel)
        {
            sources.Add(Primitives);
            sources.Add(Model);
        }

        var compilation = CSharpCompilation.Create(
            name,
            sources.Select((text, index) => CSharpSyntaxTree.ParseText(text, options, path: $"{name}{index}.cs")),
            [.. ReferenceAssemblies, .. references ?? []],
            new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );
        // The portable form has no implementation until the union generator runs: its partial constructors have no body (CS9275) and it has no
        // Value yet (CS9386). Everything else must bind.
        compilation
           .GetDiagnostics()
           .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Id is not ( "CS9275" or "CS9386" ))
           .ShouldBeEmpty();
        return compilation;
    }

    private static readonly MetadataReference[] ReferenceAssemblies =
        [.. ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! )
           .Split(Path.PathSeparator)
           .Where(path => Path.GetFileName(path) is "System.Runtime.dll" or "System.Private.CoreLib.dll" or "netstandard.dll" or "System.Threading.Tasks.dll")
           .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
}
