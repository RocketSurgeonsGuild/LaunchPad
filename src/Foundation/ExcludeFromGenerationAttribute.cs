namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Exclude the given property from the generation via launch pad source generators
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Constructor)]
public sealed class ExcludeFromGenerationAttribute : Attribute;