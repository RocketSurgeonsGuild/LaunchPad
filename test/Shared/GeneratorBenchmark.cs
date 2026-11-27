using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Tests;

/// <summary>
///     Times source generators through <see cref="GeneratorDriver" />: a cold run, a run after an edit the generator does not read, and a run
///     after an edit to its inputs. Each case is warmed up, then run in batches; the best batch is reported, for time and for allocations
///     separately, since a background build or a collection only ever makes a batch slower.
/// </summary>
/// <remarks>
///     The tests that use it are explicit: run them with <c>--treenode-filter "/*/*/*Benchmark*/*"</c>. Set
///     <c>LAUNCHPAD_GENERATOR_BENCHMARK</c> to a file path to append the table there as well.
/// </remarks>
internal static class GeneratorBenchmark
{
    private const int WarmUpRuns = 3;
    private const int Batches = 7;
    private const int RunsPerBatch = 10;

    public sealed record Measurement(string Generator, string Case, double Milliseconds, double Megabytes);

    public static IEnumerable<Measurement> Measure(
        string generator,
        Func<GeneratorDriver> createDriver,
        Compilation baseline,
        Compilation unrelatedEdit,
        Compilation inputEdit
    )
    {
        var (Milliseconds, Megabytes) = Best(() => createDriver().RunGenerators(baseline));
        yield return new(generator, "cold", Milliseconds, Megabytes);

        var warm = createDriver().RunGenerators(baseline);
        var unrelated = Best(() => warm.RunGenerators(unrelatedEdit));
        yield return new(generator, "unrelated edit", unrelated.Milliseconds, unrelated.Megabytes);

        var input = Best(() => warm.RunGenerators(inputEdit));
        yield return new(generator, "input edit", input.Milliseconds, input.Megabytes);
    }

    public static string Report(string title, IEnumerable<Measurement> measurements)
    {
        var text = new StringBuilder();
        text.AppendLine(CultureInfo.InvariantCulture, $"### {title}");
        text.AppendLine();
        text.AppendLine("| Generator | Case | Time (ms) | Allocated (MB) |");
        text.AppendLine("|---|---|---:|---:|");
        foreach (var m in measurements)
        {
            text.AppendLine(CultureInfo.InvariantCulture, $"| {m.Generator} | {m.Case} | {m.Milliseconds:0.00} | {m.Megabytes:0.00} |");
        }

        var report = text.ToString();
        Console.WriteLine(report);
        if (Environment.GetEnvironmentVariable("LAUNCHPAD_GENERATOR_BENCHMARK") is { Length: > 0 } path)
        {
            File.AppendAllText(path, report + Environment.NewLine);
        }

        return report;
    }

    private static (double Milliseconds, double Megabytes) Best(Action run)
    {
        for (var i = 0; i < WarmUpRuns; i++) run();

        var bestMilliseconds = double.MaxValue;
        var bestBytes = long.MaxValue;
        for (var batch = 0; batch < Batches; batch++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var allocatedBefore = GC.GetTotalAllocatedBytes(precise: true);
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < RunsPerBatch; i++) run();
            stopwatch.Stop();
            var allocated = GC.GetTotalAllocatedBytes(precise: true) - allocatedBefore;
            bestMilliseconds = Math.Min(bestMilliseconds, stopwatch.Elapsed.TotalMilliseconds / RunsPerBatch);
            bestBytes = Math.Min(bestBytes, allocated / RunsPerBatch);
        }

        return (bestMilliseconds, bestBytes / 1024d / 1024d);
    }
}
