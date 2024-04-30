namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Exclude the given property from the generation via launch pad source generators
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Constructor)]
public sealed class GenerationIgnoreAttribute : Attribute;