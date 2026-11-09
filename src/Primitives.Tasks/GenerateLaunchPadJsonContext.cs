using Microsoft.Build.Framework;

namespace Rocket.Surgery.LaunchPad.Primitives.Tasks;

/// <summary>
///     Writes an application's JSON context from the JSON context manifests of its referenced assemblies, before it compiles, so the
///     System.Text.Json source generator sees a <c>[JsonSerializable]</c> for every listed type.
/// </summary>
public sealed class GenerateLaunchPadJsonContext : Microsoft.Build.Utilities.Task
{
    /// <summary>The referenced assemblies (<c>@(ReferencePath)</c>).</summary>
    [Required]
    public ITaskItem[] References { get; set; } = [];

    /// <summary>The file to write.</summary>
    [Required]
    public string OutputFile { get; set; } = "";

    /// <summary>The namespace of the context, usually the project's root namespace.</summary>
    public string? Namespace { get; set; }

    /// <summary>The name of the context class.</summary>
    public string ContextName { get; set; } = "LaunchPadJsonContext";

    /// <summary>Whether a context was written: false when no referenced assembly lists a type.</summary>
    [Output]
    public bool Generated { get; set; }

    /// <inheritdoc />
    public override bool Execute()
    {
        var entries = JsonContextManifest.Read(References.Select(r => r.ItemSpec));
        if (entries.IsEmpty)
        {
            if (File.Exists(OutputFile)) File.Delete(OutputFile);
            Log.LogMessage(MessageImportance.Low, "No referenced assembly lists a JSON context type; no JSON context is generated.");
            return true;
        }

        var source = JsonContextManifest.Write(entries, Namespace, ContextName);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile))!);
        // Unchanged text keeps the file's timestamp, so the compiler doesn't rebuild for nothing.
        if (!File.Exists(OutputFile) || File.ReadAllText(OutputFile) != source) File.WriteAllText(OutputFile, source);
        Log.LogMessage(MessageImportance.Normal, $"Generated the JSON context {ContextName} with {entries.Length} types.");
        Generated = true;
        return true;
    }
}
