namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Register the options using the configuration key as the configuration root
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterOptionsConfigurationAttribute(string configurationKey) : Attribute
{
    /// <summary>
    ///     The configuration key to use
    /// </summary>
    public string ConfigurationKey { get; } = configurationKey;

    /// <summary>
    ///     The optional options name
    /// </summary>
    public string? OptionsName { get; set; }
}
