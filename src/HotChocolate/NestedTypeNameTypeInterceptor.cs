using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using HotChocolate.Types.Descriptors.Configurations;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined
/// </summary>
public class NestedTypeNameTypeInterceptor : TypeInterceptor
{
    /// <inheritdoc />
    public override void OnBeforeCompleteName(ITypeCompletionContext completionContext, TypeSystemConfiguration configuration)
    {
        if (configuration is IFilterInputTypeConfiguration { EntityType: { IsNested: true, DeclaringType: { } }, } ft)
        {
            configuration.Name = Prefixed(ft.EntityType.DeclaringType, configuration.Name);
        }
        else if (configuration is ISortInputTypeConfiguration { EntityType: { IsNested: true, DeclaringType: { } }, } st)
        {
            configuration.Name = Prefixed(st.EntityType.DeclaringType, configuration.Name);
        }
        else if (configuration is IComplexOutputTypeConfiguration { RuntimeType: { IsNested: true, DeclaringType: { } }, } cot)
        {
            configuration.Name = Prefixed(cot.RuntimeType.DeclaringType, configuration.Name);
            if (configuration.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && cot.RuntimeType.Name == "Request")
                configuration.Name = $"{cot.RuntimeType.DeclaringType.Name}{cot.RuntimeType.Name}";
        }
        else if (configuration is ITypeConfiguration { RuntimeType: { IsNested: true, DeclaringType: { } }, } ot)
        {
            configuration.Name = Prefixed(ot.RuntimeType.DeclaringType, configuration.Name);
            if (configuration.Name.EndsWith("Input", StringComparison.OrdinalIgnoreCase) && ot.RuntimeType.Name == "Request")
                configuration.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.RuntimeType.Name}";
        }
    }

    // A name that already starts with the declaring type was chosen explicitly (the API generators name CreateRocket.Request
    // CreateRocketRequest themselves), so it is left as it is.
    private static string Prefixed(Type declaringType, string name) =>
        name.StartsWith(declaringType.Name, StringComparison.Ordinal) ? name : $"{declaringType.Name}{name}";
}
