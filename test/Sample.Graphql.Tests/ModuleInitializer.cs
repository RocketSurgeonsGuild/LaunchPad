using System.Runtime.CompilerServices;
using DiffEngine;
using NodaTime;
using Argon.NodaTime;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Path = System.IO.Path;

namespace Sample.Graphql.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.AddExtraSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
        VerifierSettings.AddExtraSettings(settings => settings.Converters.Add(new GeometryConverter()));

        VerifierSettings.DisableRequireUniquePrefix();
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

    class GeometryConverter : WriteOnlyJsonConverter
    {
        private readonly WKTWriter _writer;

        public GeometryConverter()
        {
            _writer = new WKTWriter();
        }

        public override bool CanConvert(Type type)
        {
            return typeof(Geometry).IsAssignableFrom(type);
        }

        public override void Write(VerifyJsonWriter writer, object value)
        {
            writer.WriteValue(_writer.Write((Geometry)value));
        }
    }
}
