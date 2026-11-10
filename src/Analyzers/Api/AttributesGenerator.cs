using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Emits the API attributes into the compilation, once per compilation. They are emitted in post-initialization so the transport generators can bind them with ForAttributeWithMetadataName.
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
