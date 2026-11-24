using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Analyzers.Primitives;

/// <summary>
///     Reports LPAD2024 in an application that uses the domain primitives of its references with a library LaunchPad describes them to (OpenAPI,
///     HotChocolate, EF Core, Dapper), but gets none of the Primitives Joints that do it, because it doesn't reference Primitives itself.
/// </summary>
/// <remarks>
///     <para>
///         The Primitives Joints are templates written only into a project with a direct reference to Primitives, and their registry,
///         <c>LaunchPadPrimitives</c>, is one of them. Without it every domain primitive is an empty schema in OpenAPI (ASP.NET Core can't see
///         what its JSON converter writes), and HotChocolate, EF Core and Dapper can't map it. Primitives' own analyzers don't run in such an
///         application, as a reference to Primitives through a library doesn't carry them, so the check is here, in the analyzer every LaunchPad
///         application runs.
///     </para>
///     <para>
///         The rule is LPAD2024, in the block of the Primitives analyzers (src/Primitives.Analyzers), which leave the ID to it. It is reported
///         once, for the project, only in an application that isn't a test project: a library gets no templates, and its application is checked.
///     </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrimitivesReferenceAnalyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor PrimitivesNotReferenced { get; } = new(
        "LPAD2024",
        "Domain primitives without a reference to Primitives",
        "{0} uses the domain primitives of {1} with {2}, but doesn't reference Rocket.Surgery.LaunchPad.Primitives, so it gets none of the Joints that describe domain primitives to them; add a PackageReference to Rocket.Surgery.LaunchPad.Primitives",
        "LaunchPad.Primitives.Usage",
        DiagnosticSeverity.Error,
        true,
        "The Primitives Joints are written only into an application that references Rocket.Surgery.LaunchPad.Primitives itself. Without them every domain primitive is an empty schema in an OpenAPI document, and HotChocolate, Entity Framework Core and Dapper can't map one."
    );

    private const string PrimitivesAssembly = "Rocket.Surgery.LaunchPad.Primitives";

    // The registry template, written with the Joints into an application that references Primitives.
    private const string Registry = "Rocket.Surgery.LaunchPad.Primitives.Generated.LaunchPadPrimitives";

    // Every domain primitive implements it, a value object too.
    private const string DomainPrimitive = "Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive`1";

    private const string OutputTypeProperty = "build_property.OutputType";
    private const string IsTestProjectProperty = "build_property.IsTestProject";

    // The libraries a Primitives Joint (or the registry generator, for Dapper) describes domain primitives to, by the assembly the template
    // references, and as the message names them.
    private static readonly (string Assembly, string Name)[] Libraries =
    [
        ("Microsoft.AspNetCore.OpenApi", "OpenAPI"),
        ("HotChocolate.Types", "HotChocolate"),
        ("Microsoft.EntityFrameworkCore", "Entity Framework Core"),
        ("Dapper", "Dapper"),
    ];

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [PrimitivesNotReferenced];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var compilation = context.Compilation;
        var options = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;
        if (!IsApplication(compilation, options) || IsTestProject(options)) return;
        if (compilation.Assembly.GetTypeByMetadataName(Registry) is not null) return;

        var references = compilation.SourceModule.ReferencedAssemblySymbols;
        if (references.FirstOrDefault(static a => a.Name == PrimitivesAssembly)?.GetTypeByMetadataName(DomainPrimitive) is not { } domainPrimitive) return;

        var libraries = Libraries.Where(l => references.Any(a => a.Name == l.Assembly)).Select(static l => l.Name).ToArray();
        if (libraries.Length == 0) return;

        var declaring = references
                       .Where(ReferencesPrimitives)
                       .Where(a => DeclaresDomainPrimitive(a.GlobalNamespace, domainPrimitive, context.CancellationToken))
                       .Select(static a => a.Name)
                       .OrderBy(static a => a, StringComparer.Ordinal)
                       .ToArray();
        if (declaring.Length == 0) return;

        context.ReportDiagnostic(
            Diagnostic.Create(PrimitivesNotReferenced, Location.None, compilation.AssemblyName, List(declaring), List(libraries))
        );
    }

    // "A", "A and B", "A, B and C".
    private static string List(string[] items) =>
        items.Length < 2 ? string.Join("", items) : string.Join(", ", items, 0, items.Length - 1) + " and " + items[^1];

    // The project's OutputType when the build makes it visible (Foundation's targets do), otherwise what the compilation is built as.
    private static bool IsApplication(Compilation compilation, AnalyzerConfigOptions options) =>
        options.TryGetValue(OutputTypeProperty, out var outputType) && !string.IsNullOrEmpty(outputType)
            ? outputType.Equals("Exe", StringComparison.OrdinalIgnoreCase)
           || outputType.Equals("WinExe", StringComparison.OrdinalIgnoreCase)
           || outputType.Equals("AppContainerExe", StringComparison.OrdinalIgnoreCase)
            : compilation.Options.OutputKind is OutputKind.ConsoleApplication or OutputKind.WindowsApplication or OutputKind.WindowsRuntimeApplication;

    // Joinery.Dovetail makes IsTestProject visible to the compiler.
    private static bool IsTestProject(AnalyzerConfigOptions options) =>
        options.TryGetValue(IsTestProjectProperty, out var value) && bool.TryParse(value, out var isTestProject) && isTestProject;

    private static bool ReferencesPrimitives(IAssemblySymbol assembly) =>
        assembly.Modules.Any(static m => m.ReferencedAssemblySymbols.Any(static r => r.Name == PrimitivesAssembly));

    // A domain primitive the application can use: a public type, or one nested in public types, that implements IDomainPrimitive<TSelf>.
    private static bool DeclaresDomainPrimitive(INamespaceOrTypeSymbol container, INamedTypeSymbol domainPrimitive, CancellationToken cancellationToken)
    {
        foreach (var member in container.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (member)
            {
                case INamespaceSymbol ns when DeclaresDomainPrimitive(ns, domainPrimitive, cancellationToken):
                    return true;
                case INamedTypeSymbol { DeclaredAccessibility: Accessibility.Public } type:
                    if (type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, domainPrimitive))) return true;
                    if (DeclaresDomainPrimitive(type, domainPrimitive, cancellationToken)) return true;
                    break;
                default:
                    break;
            }
        }

        return false;
    }
}
