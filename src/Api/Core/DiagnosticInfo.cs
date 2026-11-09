using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     A source location that can live in a cached model: a file path and spans instead of a <see cref="Location" />.
/// </summary>
internal sealed record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public static LocationInfo? From(Location? location) =>
        location is { IsInSource: true, SourceTree: { } tree }
            ? new LocationInfo(tree.FilePath, location.SourceSpan, location.GetLineSpan().Span)
            : null;

    public static LocationInfo? From(ISymbol symbol) => From(symbol.Locations.FirstOrDefault());

    public static LocationInfo? From(AttributeData attribute) => From(attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation());

    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);
}

/// <summary>
///     A diagnostic that can live in a cached model and be reported later.
/// </summary>
internal sealed record DiagnosticInfo(string Id, LocationInfo? Location, EquatableArray<string> Arguments)
{
    public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, LocationInfo? location, params string[] arguments) =>
        new(descriptor.Id, location, arguments.ToEquatableArray());

    public Diagnostic ToDiagnostic() =>
        Diagnostic.Create(ApiDiagnostics.Find(Id), Location?.ToLocation() ?? Microsoft.CodeAnalysis.Location.None, Arguments.Cast<object>().ToArray());
}
