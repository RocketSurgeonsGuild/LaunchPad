namespace Rocket.Surgery.LaunchPad.WebAssembly.Hosting;

/// <summary>
///     RocketSerilogOptions.
/// </summary>
[PublicAPI]
public class LaunchPadLoggingOptions
{
    /// <summary>
    ///     The default console message template
    /// </summary>
    public string ConsoleMessageTemplate { get; set; } =
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message}{NewLine}{Exception}";

    /// <summary>
    ///     Enable or disable console logging, defaults to enabled
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = true;
}
