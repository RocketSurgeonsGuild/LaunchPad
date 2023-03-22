using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
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
        DefinitionBase? definition
    )
    {
        if (definition is IFilterInputTypeDefinition { EntityType: { IsNested: true, DeclaringType: { } } } ft)
        {
            definition.Name = $"{ft.EntityType.DeclaringType.Name}{definition.Name}";
        }
        else if (definition is ISortInputTypeDefinition { EntityType: { IsNested: true, DeclaringType: { } } } st)
        {
            definition.Name = $"{st.EntityType.DeclaringType.Name}{definition.Name}";
        }
        else if (definition is IComplexOutputTypeDefinition { RuntimeType: { IsNested: true, DeclaringType: { } } } cot)
        {
            definition.Name = $"{cot.RuntimeType.DeclaringType.Name}{cot.Name}";
            if (definition.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && cot.RuntimeType.Name == "Request")
            {
                definition.Name = $"{cot.RuntimeType.DeclaringType.Name}{cot.RuntimeType.Name}";
            }
        }
        else if (definition is ITypeDefinition { RuntimeType: { IsNested: true, DeclaringType: { } } } ot)
        {
            definition.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.Name}";
            if (definition.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && ot.RuntimeType.Name == "Request")
            {
                definition.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.RuntimeType.Name}";
            }
        }
    }
}
