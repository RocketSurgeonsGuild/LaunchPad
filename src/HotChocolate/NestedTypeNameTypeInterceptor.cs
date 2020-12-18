using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors.Definitions;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.HotChocolate
{
    public class NestedTypeNameTypeInterceptor : TypeInterceptor
    {
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
}