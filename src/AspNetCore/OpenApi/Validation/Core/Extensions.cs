// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Core;

/// <summary>
///     Extensions for some swagger specific work.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Is supported swagger numeric type.
    /// </summary>
    internal static bool IsNumeric(this object value)
    {
        return value is int || value is long || value is float || value is double || value is decimal;
    }

    /// <summary>
    ///     Convert numeric to double.
    /// </summary>
    internal static decimal NumericToDecimal(this object value)
    {
        return Convert.ToDecimal(value);
    }

    internal static Type? GetMemberType(this MemberInfo memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo pi => pi.PropertyType,
            FieldInfo fi    => fi.FieldType,
            _               => null
        };
    }

    /// <summary>
    ///     Returns not null enumeration.
    /// </summary>
    internal static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue>? collection)
    {
        return collection ?? Array.Empty<TValue>();
    }

    internal static IEnumerable<TValue> ToArrayDebug<TValue>(this IEnumerable<TValue>? collection)
    {
#if DEBUG
        return collection?.ToArray() ?? Array.Empty<TValue>();
#else
            return collection;
#endif
    }
}
