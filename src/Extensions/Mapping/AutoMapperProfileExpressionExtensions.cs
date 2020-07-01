using AutoMapper;
using JetBrains.Annotations;

namespace Rocket.Surgery.Extensions.AutoMapper
{
    /// <summary>
    /// AutoMapperProfileExpressionExtensions.
    /// </summary>
    [PublicAPI]
    public static class AutoMapperProfileExpressionExtensions
    {
        /// <summary>
        /// Called when [defined properties].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>T.</returns>
        public static T OnlyDefinedProperties<T>(this T configuration)
            where T : IProfileExpression
        {
            configuration.ForAllPropertyMaps(
                OnlyDefinedPropertiesMethods.ForStrings,
                OnlyDefinedPropertiesMethods.StringCondition
            );
            configuration.ForAllPropertyMaps(
                OnlyDefinedPropertiesMethods.ForValueTypes,
                OnlyDefinedPropertiesMethods.ValueTypeCondition
            );
            configuration.ForAllPropertyMaps(
                OnlyDefinedPropertiesMethods.ForNullableValueTypes,
                OnlyDefinedPropertiesMethods.NullableValueTypeCondition
            );
            return configuration;
        }
    }
}