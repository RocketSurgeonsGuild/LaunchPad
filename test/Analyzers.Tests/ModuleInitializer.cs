using System.Runtime.CompilerServices;
using Analyzers.Tests.Helpers;
using DiffEngine;
using Microsoft.CodeAnalysis;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        VerifyNodaTime.Enable();

        DiffRunner.Disabled = true;
        VerifierSettings.RegisterFileConverter<GenerationTestResult>(Convert);
        VerifierSettings.RegisterFileConverter<GenerationTestResults>(Convert);
    }

    private static ConversionResult Convert(GenerationTestResults target, IReadOnlyDictionary<string, object> context)
    {
        var targets = new List<Target>();
//        targets.AddRange(target.InputSyntaxTrees.Select(Selector));
        foreach (var item in target.Results)
        {
            targets.AddRange(item.Value.SyntaxTrees.Select(Selector));
        }

        return new(new { target.ResultDiagnostics, Results = target.Results.ToDictionary(z => z.Key.FullName!, z => z.Value.Diagnostics) }, targets);
    }

    private static Target Selector(SyntaxTree source)
    {
        var data = $@"//HintName: {source.FilePath}
{source.GetText()}";
        return new("cs.txt", data);
    }

    private static ConversionResult Convert(GenerationTestResult target, IReadOnlyDictionary<string, object> context)
    {
        return new(new { target.Diagnostics }, target.SyntaxTrees.Select(Selector));
    }
}
