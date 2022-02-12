using AutoMapper;
using AutoMapper.Internal;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     AutoMapperProfileExpressionExtensions.
/// </summary>
[PublicAPI]
public static class AutoMapperProfileExpressionExtensions
{
    /// <summary>
    ///     Called when [defined properties].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <returns>T.</returns>
    public static T OnlyDefinedProperties<T>(this T configuration)
        where T : IProfileExpression
    {
        configuration.Internal().ForAllPropertyMaps(
            OnlyDefinedPropertiesMethods.ForStrings,
            OnlyDefinedPropertiesMethods.StringCondition
        );
        configuration.Internal().ForAllPropertyMaps(
            OnlyDefinedPropertiesMethods.ForValueTypes,
            OnlyDefinedPropertiesMethods.ValueTypeCondition
        );
        configuration.Internal().ForAllPropertyMaps(
            OnlyDefinedPropertiesMethods.ForNullableValueTypes,
            OnlyDefinedPropertiesMethods.NullableValueTypeCondition
        );
        return configuration;
    }
}
