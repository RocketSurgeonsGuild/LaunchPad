using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Emits the API attributes into the compilation. Only this package emits them; every transport package depends on it, so each compilation
///     gets exactly one copy. They are emitted in post-initialization so the transport generators can bind them with ForAttributeWithMetadataName.
/// </summary>
[Generator]
public sealed class AttributesGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(
            static output =>
            {
                output.AddEmbeddedAttributeDefinition();
                output.AddSource("Rocket.Surgery.LaunchPad.Api.Attributes.g.cs", AttributeSources.Attributes);
            }
        );
}
