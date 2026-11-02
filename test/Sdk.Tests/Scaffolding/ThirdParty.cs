namespace Sdk.Tests.Scaffolding;

/// <summary>
///     The third-party packages the technology tests reference, at versions of their own: the repository pins some
///     of them only for the packages that build on them, and the tests only evaluate the projects, so nothing is restored.
/// </summary>
internal static class ThirdParty
{
    /// <summary>
    ///     NetTopologySuite, a technology of its own and the partner of LaunchPad's StrawberryShake integration.
    /// </summary>
    public const string NetTopologySuite = "NetTopologySuite";
    public const string NetTopologySuiteVersion = "2.6.0";

    /// <summary>
    ///     The StrawberryShake client package a console or server application references.
    /// </summary>
    public const string StrawberryShake = "StrawberryShake.Server";
    public const string StrawberryShakeVersion = "15.1.11";
}
