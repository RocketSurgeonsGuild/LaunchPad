using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Analyzers.Tests
{
    public record GenerationTestResults(
        CSharpCompilation InputCompilation,
        ImmutableArray<Diagnostic> InputDiagnostics,
        ImmutableArray<SyntaxTree> InputSyntaxTrees,
        ImmutableDictionary<Type, GenerationTestResult> Results
    )
    {
        public bool TryGetResult(Type type, [NotNullWhen(true)] out GenerationTestResult? result) => Results.TryGetValue(type, out result);

        public bool TryGetResult<T>([NotNullWhen(true)] out GenerationTestResult? result)
            where T : ISourceGenerator, new() => Results.TryGetValue(typeof(T), out result);

        public void EnsureDiagnosticSeverity(DiagnosticSeverity severity = DiagnosticSeverity.Warning)
        {
            Assert.Empty(InputDiagnostics.Where(x => x.Severity >= severity));
            foreach (var result in Results.Values)
            {
                result.EnsureDiagnostics(severity);
            }
        }

        public void AssertGeneratedAsExpected<T>(string expectedValue, params string[] expectedValues)
            where T : ISourceGenerator, new()
        {
            if (!TryGetResult<T>(out var result))
            {
                Assert.NotNull(result);
                return;
            }

            result.AssertGeneratedAsExpected(expectedValue);
        }
    }
}