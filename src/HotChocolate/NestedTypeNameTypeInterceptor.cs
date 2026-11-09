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
        if (definition is IFilterInputTypeDefinition { EntityType: { IsNested: true, DeclaringType: { }, }, } ft)
        {
            definition.Name = Prefixed(ft.EntityType.DeclaringType, definition.Name);
        }
        else if (definition is ISortInputTypeDefinition { EntityType: { IsNested: true, DeclaringType: { }, }, } st)
        {
            definition.Name = Prefixed(st.EntityType.DeclaringType, definition.Name);
        }
        else if (definition is IComplexOutputTypeDefinition { RuntimeType: { IsNested: true, DeclaringType: { }, }, } cot)
        {
            definition.Name = Prefixed(cot.RuntimeType.DeclaringType, cot.Name);
            if (definition.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && cot.RuntimeType.Name == "Request")
                definition.Name = $"{cot.RuntimeType.DeclaringType.Name}{cot.RuntimeType.Name}";
        }
        else if (definition is ITypeDefinition { RuntimeType: { IsNested: true, DeclaringType: { }, }, } ot)
        {
            definition.Name = Prefixed(ot.RuntimeType.DeclaringType, ot.Name);
            if (definition.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && ot.RuntimeType.Name == "Request")
                definition.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.RuntimeType.Name}";
        }
    }

    // A name that already starts with the declaring type was chosen explicitly (the API generators name CreateRocket.Request
    // CreateRocketRequest themselves), so it is left as it is.
    private static string Prefixed(Type declaringType, string name) =>
        name.StartsWith(declaringType.Name, StringComparison.Ordinal) ? name : $"{declaringType.Name}{name}";
}
