using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors.Definitions;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined
/// </summary>
public class NestedTypeNameTypeInterceptor : TypeInterceptor
{
    /// <inheritdoc />
    public override void OnBeforeCompleteName(
        ITypeCompletionContext completionContext,
        DefinitionBase? definition,
        IDictionary<string, object?> contextData
    )
    {
        if (definition is ObjectTypeDefinition ot)
        {
            if (ot.RuntimeType is { IsNested: true, DeclaringType: { } })
            {
                ot.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.Name}";
            }
        }

        if (definition is InputObjectTypeDefinition iotd)
        {
            if (iotd.RuntimeType is { IsNested: true, DeclaringType: { } })
            {
                iotd.Name = $"{iotd.RuntimeType.DeclaringType.Name}{iotd.Name}";

                if (iotd.Name.Value.EndsWith("Input") && iotd.RuntimeType.Name == "Request")
                {
                    iotd.Name = $"{iotd.RuntimeType.DeclaringType.Name}{iotd.RuntimeType.Name}";
                }
            }
        }
    }
}
