// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Core;

/// <summary>
///     Returns string equality only by symbols ignore case.
///     It can be used for comparing camelCase, PascalCase, snake_case, kebab-case identifiers.
/// </summary>
public class IgnoreAllStringComparer : StringComparer
{
    /// <summary>
    ///     The default comparer instance
    /// </summary>
    public static readonly StringComparer Instance = new IgnoreAllStringComparer();

    internal static bool GetNextSymbol(string? value, ref int startIndex, out char symbol)
    {
        while (startIndex >= 0 && startIndex < value?.Length)
        {
            var current = value[startIndex++];
            if (char.IsLetterOrDigit(current))
            {
                symbol = char.ToUpperInvariant(current);
                return true;
            }
        }

        startIndex = -1;
        symbol = char.MinValue;
        return false;
    }

    /// <inheritdoc />
    public override int Compare(string? left, string? right)
    {
        var leftIndex = 0;
        var rightIndex = 0;
        var compare = 0;
        while (true)
        {
            GetNextSymbol(left, ref leftIndex, out var leftSymbol);
            GetNextSymbol(right, ref rightIndex, out var rightSymbol);

            compare = leftSymbol.CompareTo(rightSymbol);
            if (compare != 0 || leftIndex < 0 || rightIndex < 0)
            {
                break;
            }
        }

        return compare;
    }

    /// <inheritdoc />
    public override bool Equals(string? left, string? right)
    {
        if (left == null || right == null)
            return false;

        var leftIndex = 0;
        var rightIndex = 0;
        bool equals;
        while (true)
        {
            var hasLeftSymbol = GetNextSymbol(left, ref leftIndex, out var leftSymbol);
            var hasRightSymbol = GetNextSymbol(right, ref rightIndex, out var rightSymbol);

            equals = leftSymbol == rightSymbol;
            if (!equals || !hasLeftSymbol || !hasRightSymbol)
            {
                break;
            }
        }

        return equals;
    }

    /// <inheritdoc />
    public override int GetHashCode(string? obj)
    {
        unchecked
        {
            var index = 0;
            var hash = 0;
            while (GetNextSymbol(obj, ref index, out var symbol))
            {
                hash = ( 31 * hash ) + char.ToUpperInvariant(symbol).GetHashCode();
            }

            return hash;
        }
    }
}
