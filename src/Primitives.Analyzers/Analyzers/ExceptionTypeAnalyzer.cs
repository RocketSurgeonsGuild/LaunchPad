using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     LPAD2017: the exception the project's <c>LaunchPadPrimitivesExceptionType</c> names, which the generated <c>Create</c> throws for a
///     rejected value, must exist, derive from <see cref="Exception" /> and have a constructor that takes the message. The name is read the way
///     the generator writes it, <c>new global::&lt;name&gt;(message)</c>: a C# name (a nested type is <c>Outer.Inner</c>), bound across the
///     project and its references, where two references that both define it make it ambiguous. Checked once, when the project declares a
///     domain primitive.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [AnalyzerDiagnostics.InvalidExceptionType];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                if (!start.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(PrimitiveSymbols.ExceptionTypeProperty, out var name)
                 || string.IsNullOrWhiteSpace(name))
                {
                    return;
                }

                name = name.Trim();
                if (Problem(start.Compilation, name) is not { } problem) return;

                var declaresPrimitive = 0;
                start.RegisterSymbolAction(
                    c =>
                    {
                        if (c.Symbol is INamedTypeSymbol type && PrimitiveSymbols.GetDeclaration(type) is { }) Interlocked.Exchange(ref declaresPrimitive, 1);
                    },
                    SymbolKind.NamedType
                );
                start.RegisterCompilationEndAction(
                    end =>
                    {
                        if (Volatile.Read(ref declaresPrimitive) == 0) return;
                        end.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.InvalidExceptionType, Location.None, name, problem));
                    }
                );
            }
        );
    }

    /// <summary>What is wrong with the exception type <paramref name="name" />, to end the message; null when nothing is.</summary>
    internal static string? Problem(Compilation compilation, string name)
    {
        var typeName = name.StartsWith("global::", StringComparison.Ordinal) ? name["global::".Length..] : name;
        if (typeName.IndexOfAny(['+', '`', '<', '>', ',', ' ']) >= 0)
        {
            return "is not a C# name of a type: write a nested type as Outer.Inner (the generator writes new global::<name>(message)), and a generic type can't be used";
        }

        var candidates = Resolve(compilation, typeName);
        if (candidates.IsEmpty) return "the project cannot find";
        var accessible = candidates.Where(c => compilation.IsSymbolAccessibleWithin(c, compilation.Assembly)).ToImmutableArray();
        if (accessible.IsEmpty) return "the project cannot access";
        if (Choose(compilation, accessible) is not { } type)
        {
            var assemblies = string.Join(", ", accessible.Select(c => c.ContainingAssembly.Name).Distinct().OrderBy(n => n, StringComparer.Ordinal));
            return $"is defined in more than one referenced assembly ({assemblies}), so the name is ambiguous";
        }

        var derives = false;
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current is { Name: "Exception", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } })
            {
                derives = true;
                break;
            }
        }

        if (!derives) return "does not derive from System.Exception";
        if (type.IsAbstract) return "is abstract, so it cannot be thrown";
        var hasMessageConstructor = type.InstanceConstructors.Any(
            c => c.Parameters is [{ Type.SpecialType: SpecialType.System_String }] && compilation.IsSymbolAccessibleWithin(c, compilation.Assembly)
        );
        return hasMessageConstructor ? null : "has no accessible constructor that takes the message (a single string)";
    }

    /// <summary>
    ///     The types the C# name <paramref name="name" /> (<c>Namespace.Outer.Inner</c>) can mean, as <c>global::</c> + the name binds in the
    ///     generated code: each part is a namespace or a type of the one before, across the compilation and its references.
    /// </summary>
    private static ImmutableArray<INamedTypeSymbol> Resolve(Compilation compilation, string name)
    {
        var current = new List<INamespaceOrTypeSymbol> { compilation.GlobalNamespace };
        foreach (var part in name.Split('.'))
        {
            if (part.Length == 0) return [];
            var next = new List<INamespaceOrTypeSymbol>();
            foreach (var container in current)
            {
                foreach (var member in container.GetMembers(part))
                {
                    if (member is INamespaceSymbol or INamedTypeSymbol { Arity: 0 }) next.Add((INamespaceOrTypeSymbol)member);
                }
            }

            current = next;
        }

        return [.. current.OfType<INamedTypeSymbol>().Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)];
    }

    /// <summary>
    ///     The type the name binds to among several the project can access, as the compiler chooses: one the project declares wins over its
    ///     references (CS0436 is a warning); otherwise one in the core library; otherwise none, the name is ambiguous (CS0433).
    /// </summary>
    private static INamedTypeSymbol? Choose(Compilation compilation, ImmutableArray<INamedTypeSymbol> types)
    {
        if (types.Length == 1) return types[0];
        var own = types.Where(t => SymbolEqualityComparer.Default.Equals(t.ContainingAssembly, compilation.Assembly)).ToArray();
        if (own.Length == 1) return own[0];
        var core = types.Where(t => SymbolEqualityComparer.Default.Equals(t.ContainingAssembly, compilation.ObjectType.ContainingAssembly)).ToArray();
        return core.Length == 1 ? core[0] : null;
    }
}
