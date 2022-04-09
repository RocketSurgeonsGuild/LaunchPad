using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Analyzers.Tests.Helpers;

public record GenerationTestResults(
    CSharpCompilation InputCompilation,
    ImmutableArray<Diagnostic> InputDiagnostics,
    ImmutableArray<SyntaxTree> InputSyntaxTrees,
    ImmutableDictionary<Type, GenerationTestResult> Results,
    ImmutableArray<Diagnostic> ResultDiagnostics
)
{
    public static implicit operator CSharpCompilation(GenerationTestResults results)
    {
        return results.ToFinalCompilation();
    }

    public bool TryGetResult(Type type, [NotNullWhen(true)] out GenerationTestResult? result)
    {
        return Results.TryGetValue(type, out result);
    }

    public bool TryGetResult<T>([NotNullWhen(true)] out GenerationTestResult? result)
        where T : new()
    {
        return Results.TryGetValue(typeof(T), out result);
    }

    public void EnsureDiagnosticSeverity(DiagnosticSeverity severity = DiagnosticSeverity.Warning)
    {
        Assert.Empty(InputDiagnostics.Where(x => x.Severity >= severity));
        foreach (var result in Results.Values)
        {
            result.EnsureDiagnostics(severity);
        }
    }

    public void AssertGeneratedAsExpected<T>(string expectedValue, params string[] expectedValues)
        where T : new()
    {
        if (!TryGetResult<T>(out var result))
        {
            Assert.NotNull(result);
            return;
        }

        result.AssertGeneratedAsExpected(expectedValue);
    }

    public void AssertCompilationWasSuccessful()
    {
        Assert.Empty(
            InputDiagnostics
               .Where(z => !z.GetMessage().Contains("does not contain a definition for"))
               .Where(z => !z.GetMessage().Contains("Assuming assembly reference"))
               .Where(x => x.Severity >= DiagnosticSeverity.Warning)
        );
        foreach (var result in Results.Values)
        {
            result.EnsureDiagnosticSeverity();
        }
    }

    public void AssertGenerationWasSuccessful()
    {
        foreach (var item in Results.Values)
        {
            Assert.NotNull(item.Compilation);
            item.EnsureDiagnosticSeverity();
        }
    }

    public CSharpCompilation ToFinalCompilation()
    {
        return Results.Values.Aggregate(InputCompilation, (current, item) => current.AddSyntaxTrees(item.SyntaxTrees));
    }
}
