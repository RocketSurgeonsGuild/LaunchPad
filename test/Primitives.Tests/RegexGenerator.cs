using Microsoft.CodeAnalysis;

namespace Primitives.Tests;

/// <summary>
///     The regular expression generator of the .NET SDK, which implements the <c>[GeneratedRegex]</c> members of a domain primitive, loaded from
///     the targeting pack of the running runtime, where a build finds it.
/// </summary>
internal static class RegexGenerator
{
    private const string AssemblyFile = "System.Text.RegularExpressions.Generator.dll";

    /// <summary>The generator's type, for the harness.</summary>
    public static Type Type { get; } = Load();

    /// <summary>A new instance of the generator, for a driver.</summary>
    public static IIncrementalGenerator Create() => (IIncrementalGenerator)Activator.CreateInstance(Type)!;

    private static Type Load()
    {
        // dotnet/shared/Microsoft.NETCore.App/{version} beside dotnet/packs/Microsoft.NETCore.App.Ref/{version}.
        var runtime = new DirectoryInfo(Path.GetDirectoryName(typeof(object).Assembly.Location)!);
        var packs = new DirectoryInfo(Path.Combine(runtime.Parent!.Parent!.Parent!.FullName, "packs", "Microsoft.NETCore.App.Ref"));
        var pack = new[] { new DirectoryInfo(Path.Combine(packs.FullName, runtime.Name)) }
                  .Concat(packs.Exists ? packs.EnumerateDirectories($"{Environment.Version.Major}.*").OrderByDescending(z => z.Name, StringComparer.Ordinal) : [])
                  .Select(z => Path.Combine(z.FullName, "analyzers", "dotnet", "cs", AssemblyFile))
                  .FirstOrDefault(File.Exists)
         ?? throw new InvalidOperationException($"{AssemblyFile} is in no targeting pack under {packs.FullName}.");
        return System.Reflection.Assembly.LoadFrom(pack).GetType("System.Text.RegularExpressions.Generator.RegexGenerator", true)!;
    }
}
