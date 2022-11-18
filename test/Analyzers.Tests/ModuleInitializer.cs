using System.Runtime.CompilerServices;
using Analyzers.Tests.Helpers;
using DiffEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.RegisterFileConverter<GenerationTestResult>(Convert);
        VerifierSettings.RegisterFileConverter<GenerationTestResults>(Convert);
        VerifierSettings.RegisterFileConverter<GeneratorDriver>(Convert);
        VerifierSettings.RegisterFileConverter<GeneratorDriverRunResult>(Convert);
//        VerifySourceGenerators.Enable();

        VerifierSettings.AddExtraSettings(
            serializer =>
            {
                var converters = serializer.Converters;
                converters.Add(new LocalizableStringConverter());
                converters.Add(new DiagnosticConverter());
                converters.Add(new LocationConverter());
                converters.Add(new GeneratedSourceResultConverter());
                converters.Add(new DiagnosticDescriptorConverter());
                converters.Add(new SourceTextConverter());
            }
        );

        DiffRunner.Disabled = true;
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type)
                {
                    return type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
                }

                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
                return new(path, typeName, method.Name);
            }
        );
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
        var hintPath = source.FilePath
                             .Replace("\\", "/", StringComparison.OrdinalIgnoreCase)
                             .Replace(".roslyn4.0", string.Empty, StringComparison.OrdinalIgnoreCase)
                             .Replace(".roslyn4.4", string.Empty, StringComparison.OrdinalIgnoreCase);
        var data = $@"//HintName: {hintPath}
{source.GetText()}";
        return new("cs.txt", data.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase));
    }

    private static ConversionResult Convert(GenerationTestResult target, IReadOnlyDictionary<string, object> context)
    {
        return new(new { target.Diagnostics }, target.SyntaxTrees.Select(Selector));
    }


    private static ConversionResult Convert(GeneratorDriverRunResult target, IReadOnlyDictionary<string, object> context)
    {
        var exceptions = new List<Exception>();
        var targets = new List<Target>();
        foreach (var result in target.Results)
        {
            if (result.Exception != null)
            {
                exceptions.Add(result.Exception);
            }

            var collection = result.GeneratedSources
                                   .OrderBy(x => x.HintName)
                                   .Select(SourceToTarget);
            targets.AddRange(collection);
        }

        if (exceptions.Count == 1)
        {
            throw exceptions.First();
        }

        if (exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }

        if (target.Diagnostics.Any())
        {
            var info = new
            {
                target.Diagnostics
            };
            return new(info, targets);
        }

        return new(null, targets);
    }

    private static Target SourceToTarget(GeneratedSourceResult source)
    {
        var hintName = source.HintName
                             .Replace(".roslyn4.0", string.Empty, StringComparison.OrdinalIgnoreCase)
                             .Replace(".roslyn4.4", string.Empty, StringComparison.OrdinalIgnoreCase);
        var data = $@"//HintName: {hintName}
{source.SourceText}";
        return new("cs", data, Path.GetFileNameWithoutExtension(hintName));
    }

    private static ConversionResult Convert(GeneratorDriver target, IReadOnlyDictionary<string, object> context)
    {
        return Convert(target.GetRunResult(), context);
    }

    private class LocalizableStringConverter :
        WriteOnlyJsonConverter<LocalizableString>
    {
        public override void Write(VerifyJsonWriter writer, LocalizableString value)
        {
            writer.WriteValue(value.ToString());
        }
    }

    private class DiagnosticConverter :
        WriteOnlyJsonConverter<Diagnostic>
    {
        public override void Write(VerifyJsonWriter writer, Diagnostic value)
        {
            writer.WriteStartObject();
            writer.WriteMember(value, value.Id, "Id");
            var descriptor = value.Descriptor;
            writer.WriteMember(value, descriptor.Title.ToString(), "Title");
            writer.WriteMember(value, value.Severity.ToString(), "Severity");
            writer.WriteMember(value, value.WarningLevel, "WarningLevel");
            writer.WriteMember(value, value.Location.GetMappedLineSpan().ToString(), "Location");
            writer.WriteMember(value, descriptor.Description.ToString(), "Description");
            writer.WriteMember(value, descriptor.HelpLinkUri, "HelpLink");
            writer.WriteMember(value, descriptor.MessageFormat.ToString(), "MessageFormat");
            writer.WriteMember(value, value.GetMessage(), "Message");
            writer.WriteMember(value, descriptor.Category, "Category");
            writer.WriteMember(value, descriptor.CustomTags, "CustomTags");
            writer.WriteEndObject();
        }
    }

    private class LocationConverter :
        WriteOnlyJsonConverter<Location>
    {
        public override void Write(VerifyJsonWriter writer, Location value)
        {
            writer.WriteValue(value.GetMappedLineSpan().ToString());
        }
    }

    private class GeneratedSourceResultConverter :
        WriteOnlyJsonConverter<GeneratedSourceResult>
    {
        public override void Write(VerifyJsonWriter writer, GeneratedSourceResult value)
        {
            writer.WriteStartObject();
            writer.WriteMember(
                value, value.HintName
                            .Replace(".roslyn4.0", string.Empty, StringComparison.OrdinalIgnoreCase)
                            .Replace(".roslyn4.4", string.Empty, StringComparison.OrdinalIgnoreCase), "HintName"
            );
            writer.WriteMember(value, value.SourceText, "Source");
            writer.WriteEndObject();
        }
    }

    private class DiagnosticDescriptorConverter :
        WriteOnlyJsonConverter<DiagnosticDescriptor>
    {
        public override void Write(VerifyJsonWriter writer, DiagnosticDescriptor value)
        {
            writer.WriteStartObject();
            writer.WriteMember(value, value.Id, "Id");
            writer.WriteMember(value, value.Title.ToString(), "Title");
            writer.WriteMember(value, value.Description.ToString(), "Description");
            writer.WriteMember(value, value.HelpLinkUri, "HelpLink");
            writer.WriteMember(value, value.MessageFormat.ToString(), "MessageFormat");
            writer.WriteMember(value, value.Category, "Category");
            writer.WriteMember(value, value.DefaultSeverity, "DefaultSeverity");
            writer.WriteMember(value, value.IsEnabledByDefault, "IsEnabledByDefault");
            writer.WriteMember(value, value.CustomTags, "CustomTags");
            writer.WriteEndObject();
        }
    }

    private class SourceTextConverter :
        WriteOnlyJsonConverter<SourceText>
    {
        public override void Write(VerifyJsonWriter writer, SourceText value)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
