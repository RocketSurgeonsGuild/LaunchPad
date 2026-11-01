namespace Sdk.Tests.Scaffolding;

/// <summary>
///     The third-party packages the Newtonsoft.Json and NetTopologySuite tests reference, at versions of their own: the repository pins some
///     of them only for the packages that build on them, and the tests only evaluate the projects, so nothing is restored.
/// </summary>
internal static class ThirdParty
{
    public const string NewtonsoftJson = "Newtonsoft.Json";
    public const string NewtonsoftJsonVersion = "13.0.4";

    public const string NetTopologySuite = "NetTopologySuite";
    public const string NetTopologySuiteVersion = "2.6.0";

    /// <summary>
    ///     Mapperly, the partner of LaunchPad's mapping.
    /// </summary>
    public const string Mapperly = "Riok.Mapperly";
    public const string MapperlyVersion = "4.2.1";

    /// <summary>
    ///     The StrawberryShake client package a console or server application references.
    /// </summary>
    public const string StrawberryShake = "StrawberryShake.Server";
    public const string StrawberryShakeVersion = "15.1.11";
}
