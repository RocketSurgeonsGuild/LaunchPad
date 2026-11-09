using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Tasks;

namespace Analyzers.Tests;

// The core of the MSBuild task that writes an application's JSON context: reading the manifests of the referenced assemblies.
public class JsonContextManifestTaskTests
{
    [Test]
    [Arguments("Sample.Core.RocketModel, Sample.Core, Version=1.0.0.0", "global::Sample.Core.RocketModel", "RocketModel")]
    [Arguments("Sample.Core.CreateRocket+Request, Sample.Core", "global::Sample.Core.CreateRocket.Request", "CreateRocketRequest")]
    [Arguments("Sample.Core.RocketModel[], Sample.Core", "global::Sample.Core.RocketModel[]", "RocketModelArray")]
    [Arguments(
        "System.Collections.Generic.List`1[[Sample.Core.RocketModel, Sample.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib",
        "global::System.Collections.Generic.List<global::Sample.Core.RocketModel>",
        "ListOfRocketModel"
    )]
    [Arguments(
        "System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib],[Sample.Core.RocketModel, Sample.Core]]",
        "global::System.Collections.Generic.Dictionary<global::System.String, global::Sample.Core.RocketModel>",
        "DictionaryOfStringAndRocketModel"
    )]
    [Arguments("Sample.Core.Outer`1+Inner`1[[System.Int32],[System.String]]", "global::Sample.Core.Outer<global::System.Int32>.Inner<global::System.String>", "OuterInnerOfInt32AndString")]
    public void Should_Read_Serialized_Type_Names(string serialized, string typeName, string propertyName) =>
        SerializedTypeName.Parse(serialized).ShouldBe((typeName, propertyName));

    [Test]
    public void Should_Skip_A_Name_It_Cannot_Read() => SerializedTypeName.Parse("System.Collections.Generic.List`1[[").ShouldBeNull();

    [Test]
    public async Task Should_Write_The_Context_From_The_Manifests_Of_The_References()
    {
        var directory = Directory.CreateTempSubdirectory("launchpad-json-context-");
        try
        {
            var contracts = Emit(
                directory,
                "Contracts",
                """
                [assembly: Rocket.Surgery.LaunchPad.Primitives.JsonContextType(typeof(Sample.Core.RocketModel))]
                [assembly: Rocket.Surgery.LaunchPad.Primitives.JsonContextType(typeof(Sample.Core.CreateRocket.Request))]
                [assembly: Rocket.Surgery.LaunchPad.Primitives.JsonContextType(typeof(System.Collections.Generic.List<Sample.Core.RocketModel>))]
                [assembly: Rocket.Surgery.LaunchPad.Primitives.JsonContextType(typeof(Sample.Core.Request), TypeInfoPropertyName = "RootRequest")]
                namespace Sample.Core
                {
                    public record RocketModel(string Name);
                    public record Request;
                    public static class CreateRocket { public record Request(string Name); }
                }
                """
            );
            // A second assembly lists a type with the same simple name, and one the first already lists.
            var other = Emit(
                directory,
                "Other",
                """
                [assembly: Rocket.Surgery.LaunchPad.Primitives.JsonContextType(typeof(Other.RocketModel))]
                namespace Other { public record RocketModel(int Id); }
                """
            );
            // The framework assemblies carry no manifest and are skipped without being decoded.
            var references = new[] { contracts, other, typeof(object).Assembly.Location, typeof(JsonContextTypeAttribute).Assembly.Location };

            var entries = JsonContextManifest.Read(references);

            await Verify(JsonContextManifest.Write(entries, "Sample.App", "LaunchPadJsonContext"), "cs")
               .UseFileName($"{nameof(JsonContextManifestTaskTests)}.{nameof(Should_Write_The_Context_From_The_Manifests_Of_The_References)}");
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Test]
    public void Should_Find_Nothing_Without_Manifests() => JsonContextManifest.Read([typeof(object).Assembly.Location, "missing.dll"]).ShouldBeEmpty();

    private static string Emit(DirectoryInfo directory, string name, string source)
    {
        var references = ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! )
                        .Split(Path.PathSeparator)
                        .Where(path => Path.GetFileName(path) is "System.Runtime.dll" or "System.Private.CoreLib.dll" or "netstandard.dll" or "System.Collections.dll")
                        .Append(typeof(JsonContextTypeAttribute).Assembly.Location)
                        .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            name,
            [CSharpSyntaxTree.ParseText(source, new(LanguageVersion.Latest))],
            references,
            new(OutputKind.DynamicallyLinkedLibrary)
        );
        var path = Path.Combine(directory.FullName, name + ".dll");
        var result = compilation.Emit(path);
        result.Success.ShouldBeTrue(string.Join("\n", result.Diagnostics));
        return path;
    }
}
