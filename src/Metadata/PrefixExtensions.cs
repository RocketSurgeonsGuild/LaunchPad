using System.Globalization;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.Metadata;

/// <summary>
///     PrefixExtensions.
/// </summary>
internal static class PrefixExtensions
{
    /// <summary>
    ///     Infers the specified instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="provider">The provider.</param>
    /// <param name="instance">The instance.</param>
    internal static void Infer<T>(this AssemblyMetadataProvider provider, T instance) where T : notnull
    {
        foreach (var property in instance.GetType().GetTypeInfo().DeclaredProperties)
        {
            // simple props only
            if (
                property.PropertyType.GetTypeInfo().IsPrimitive
             || property.PropertyType.GetTypeInfo().IsEnum
             || property.PropertyType == typeof(string))
            {
                var prefix = property.GetCustomAttribute<PrefixAttribute>()?.Key ?? string.Empty;
                var value = provider.GetValue(prefix + property.Name).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value)) property.SetValue(instance, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
            }
        }
    }
}
