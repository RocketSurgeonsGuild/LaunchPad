using System.Runtime.CompilerServices;

using DiffEngine;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

using VerifyTests.DiffPlex;
using Path = System.IO.Path;

namespace Sample.Graphql.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.DontScrubDateTimes();
        //        VerifierSettings.AddExtraSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
        VerifierSettings.AddExtraSettings(settings => settings.Converters.Add(new GeometryConverter()));
        VerifierSettings.IgnoreMember<IOperationResult>(z => z.DataInfo);
        VerifierSettings.IgnoreMember<IOperationResult>(z => z.DataFactory);

        VerifierSettings.DisableRequireUniquePrefix();
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;

                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
                return new(path, typeName, method.Name);
            }
        );
    }

    private class GeometryConverter : WriteOnlyJsonConverter
    {
        public GeometryConverter() => _writer = new();

        public override bool CanConvert(Type type) => typeof(Geometry).IsAssignableFrom(type);

        public override void Write(VerifyJsonWriter writer, object value) => writer.WriteValue(_writer.Write((Geometry)value));

        private readonly WKTWriter _writer;
    }
}
