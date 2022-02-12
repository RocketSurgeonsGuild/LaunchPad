using System.Reflection;
using AutoMapper;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     OnlyDefinedPropertiesMethods.
/// </summary>
internal static class OnlyDefinedPropertiesMethods
{
    /// <summary>
    ///     Fors the strings.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ForStrings(PropertyMap map)
    {
        if (map.SourceType == typeof(string) && map.DestinationType == typeof(string))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Strings the condition.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="expression">The expression.</param>
    public static void StringCondition(PropertyMap map, IMemberConfigurationExpression expression)
    {
        expression.Condition(
            (_, _, sourceValue, _, _) =>
            {
                if (!string.IsNullOrWhiteSpace((string)sourceValue))
                {
                    return true;
                }

                return false;
            }
        );
    }

    /// <summary>
    ///     Fors the value types.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ForValueTypes(PropertyMap map)
    {
        if (map.SourceType == null)
        {
            return false;
        }

        var source = map.SourceType.GetTypeInfo();
        var destination = map.DestinationType.GetTypeInfo();
        if (!source.IsEnum && source.IsValueType && destination.IsValueType)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Values the type condition.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="expression">The expression.</param>
    public static void ValueTypeCondition(PropertyMap map, IMemberConfigurationExpression expression)
    {
        var defaultValue = Activator.CreateInstance(map.SourceType);
        expression.Condition(
            (_, _, sourceValue, _, _) =>
            {
                if (!Equals(defaultValue, sourceValue))
                {
                    return true;
                }

                return false;
            }
        );
    }

    /// <summary>
    ///     Fors the nullable value types.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ForNullableValueTypes(PropertyMap map)
    {
        if (map.SourceType == null)
        {
            return false;
        }

        var source = Nullable.GetUnderlyingType(map.SourceType)?.GetTypeInfo();
        var destination = Nullable.GetUnderlyingType(map.DestinationType)?.GetTypeInfo();
        if (source == null || destination == null)
        {
            return false;
        }

        if (!source.IsEnum && source.IsValueType && destination.IsValueType)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Nullables the value type condition.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="expression">The expression.</param>
    public static void NullableValueTypeCondition(PropertyMap map, IMemberConfigurationExpression expression)
    {
        expression.Condition(
            (_, _, sourceValue, _, _) =>
            {
                if (sourceValue != null)
                {
                    return true;
                }

                return false;
            }
        );
    }
}
