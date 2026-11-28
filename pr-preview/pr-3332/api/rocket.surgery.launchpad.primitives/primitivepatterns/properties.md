# PrimitivePatterns Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitivePatterns](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitivepatterns.md)
- Kind: `Properties`
- Members: `12`

The patterns of the primitive kinds, which the code the generator writes for a kind matches the value against.

<a id="email"></a>

## Email

- Name: `Email`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L33)

```csharp
public static Regex Email { get; }
```

## Remarks

Pattern:

```csharp
^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [!#-'*+\--9=?A-Z^-~] atomically at least once.
○ Match '@'.
○ Match an ASCII letter or digit.
○ Optional (greedy).
    ○ Match a character in the set [\-0-9A-Za-z] greedily at most 61 times.
    ○ Match an ASCII letter or digit.
○ Loop greedily any number of times.
    ○ Match '.'.
    ○ Match an ASCII letter or digit.
    ○ Optional (greedy).
        ○ Match a character in the set [\-0-9A-Za-z] greedily at most 61 times.
        ○ Match an ASCII letter or digit.
○ Match if at the end of the string or if before an ending newline.
```

<a id="hexcolor"></a>

## HexColor

- Name: `HexColor`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L150)

```csharp
public static Regex HexColor { get; }
```

## Remarks

Pattern:

```csharp
^#([0-9A-F]{3}|[0-9A-F]{4}|[0-9A-F]{6}|[0-9A-F]{8})$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match '#'.
○ 1st capture group.
    ○ Match with 4 alternative expressions.
        ○ Match an uppercase hexadecimal digit exactly 3 times.
        ○ Match an uppercase hexadecimal digit exactly 4 times.
        ○ Match an uppercase hexadecimal digit exactly 6 times.
        ○ Match an uppercase hexadecimal digit exactly 8 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="iban"></a>

## Iban

- Name: `Iban`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L316)

```csharp
public static Regex Iban { get; }
```

## Remarks

Pattern:

```csharp
^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [A-Z] exactly 2 times.
○ Match a character in the set [0-9] exactly 2 times.
○ Match a character in the set [0-9A-Z] atomically at least 11 and at most 30 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="ipaddress"></a>

## IPAddress

- Name: `IPAddress`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L267)

```csharp
public static Regex IPAddress { get; }
```

## Remarks

Pattern:

```csharp
^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,7}:$|^:(?::[0-9a-fA-F]{1,4}){1,7}$|^(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}$|^(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}$|^(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:(?::[0-9a-fA-F]{1,4}){1,6}$|^::$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match with 11 alternative expressions, atomically.
    ○ Match a sequence of expressions.
        ○ Loop exactly 3 times.
            ○ Match with 2 alternative expressions.
                ○ Match a sequence of expressions.
                    ○ Match '2'.
                    ○ Match with 2 alternative expressions.
                        ○ Match a sequence of expressions.
                            ○ Match '5'.
                            ○ Match a character in the set [0-5].
                        ○ Match a sequence of expressions.
                            ○ Match a character in the set [0-4].
                            ○ Match a character in the set [0-9].
                ○ Match a sequence of expressions.
                    ○ Match a character in the set [01] greedily, optionally.
                    ○ Match a character in the set [0-9] atomically at least 1 and at most 2 times.
            ○ Match '.'.
        ○ Match with 2 alternative expressions.
            ○ Match a sequence of expressions.
                ○ Match '2'.
                ○ Match with 2 alternative expressions.
                    ○ Match a sequence of expressions.
                        ○ Match '5'.
                        ○ Match a character in the set [0-5].
                    ○ Match a sequence of expressions.
                        ○ Match a character in the set [0-4].
                        ○ Match a character in the set [0-9].
            ○ Match a sequence of expressions.
                ○ Match a character in the set [01] greedily, optionally.
                ○ Match a character in the set [0-9] atomically at least 1 and at most 2 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop exactly 7 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 7 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Match ':'.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Match ':'.
        ○ Loop greedily at least 1 and at most 7 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 6 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Match ':'.
        ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 5 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Loop greedily at least 1 and at most 2 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 4 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Loop greedily at least 1 and at most 3 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 3 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Loop greedily at least 1 and at most 4 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Loop greedily at least 1 and at most 2 times.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
            ○ Match ':'.
        ○ Loop greedily at least 1 and at most 5 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match ':'.
        ○ Loop greedily at least 1 and at most 6 times.
            ○ Match ':'.
            ○ Match a hexadecimal digit atomically at least 1 and at most 4 times.
        ○ Match if at the end of the string or if before an ending newline.
    ○ Match a sequence of expressions.
        ○ Match the string "::".
        ○ Match if at the end of the string or if before an ending newline.
```

<a id="isbn"></a>

## Isbn

- Name: `Isbn`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L378)

```csharp
public static Regex Isbn { get; }
```

## Remarks

Pattern:

```csharp
^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Optional (greedy).
    ○ Match the string "ISBN".
    ○ Optional (greedy).
        ○ Match the string "-1".
        ○ Match a character in the set [03].
    ○ Match ':' atomically, optionally.
    ○ Match ' '.
○ Zero-width positive lookahead.
    ○ Match with 4 alternative expressions, atomically.
        ○ Match a sequence of expressions.
            ○ Match a character in the set [0-9X] exactly 10 times.
            ○ Match if at the end of the string or if before an ending newline.
        ○ Match a sequence of expressions.
            ○ Zero-width positive lookahead.
                ○ Loop exactly 3 times.
                    ○ Match a character in the set [0-9] atomically at least once.
                    ○ Match a character in the set [ \-].
            ○ Match a character in the set [ \-0-9X] exactly 13 times.
            ○ Match if at the end of the string or if before an ending newline.
        ○ Match a sequence of expressions.
            ○ Match the string "97".
            ○ Match a character in the set [89].
            ○ Match a character in the set [0-9] exactly 10 times.
            ○ Match if at the end of the string or if before an ending newline.
        ○ Match a sequence of expressions.
            ○ Zero-width positive lookahead.
                ○ Loop exactly 4 times.
                    ○ Match a character in the set [0-9] atomically at least once.
                    ○ Match a character in the set [ \-].
            ○ Match a character in the set [ \-0-9] exactly 17 times.
            ○ Match if at the end of the string or if before an ending newline.
○ Optional (greedy).
    ○ Match the string "97".
    ○ Match a character in the set [89].
    ○ Match a character in the set [ \-] atomically, optionally.
○ Match a character in the set [0-9] greedily at least 1 and at most 5 times.
○ Match a character in the set [ \-] atomically, optionally.
○ Match a character in the set [0-9] greedily at least once.
○ Match a character in the set [ \-] atomically, optionally.
○ Match a character in the set [0-9] greedily at least once.
○ Match a character in the set [ \-] atomically, optionally.
○ Match a character in the set [0-9X].
○ Match if at the end of the string or if before an ending newline.
```

<a id="languagecode"></a>

## LanguageCode

- Name: `LanguageCode`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L102)

```csharp
public static Regex LanguageCode { get; }
```

## Remarks

Pattern:

```csharp
^[a-z]{2}$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [a-z] exactly 2 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="macaddress"></a>

## MacAddress

- Name: `MacAddress`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L293)

```csharp
public static Regex MacAddress { get; }
```

## Remarks

Pattern:

```csharp
^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Loop exactly 5 times.
    ○ 1st capture group.
        ○ Match an uppercase hexadecimal digit exactly 2 times.
        ○ Match a character in the set [\-:].
○ 2nd capture group.
    ○ Match an uppercase hexadecimal digit exactly 2 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="phone"></a>

## Phone

- Name: `Phone`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L56)

```csharp
public static Regex Phone { get; }
```

## Remarks

Pattern:

```csharp
^\+[1-9]\d{1,14}$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match '+'.
○ Match a character in the set [1-9].
○ Match a Unicode digit atomically at least 1 and at most 14 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="slug"></a>

## Slug

- Name: `Slug`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L81)

```csharp
public static Regex Slug { get; }
```

## Remarks

Pattern:

```csharp
^[a-z0-9]+(-[a-z0-9]+)*$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [0-9a-z] greedily at least once.
○ Loop greedily any number of times.
    ○ 1st capture group.
        ○ Match '-'.
        ○ Match a character in the set [0-9a-z] atomically at least once.
○ Match if at the end of the string or if before an ending newline.
```

<a id="username"></a>

## Username

- Name: `Username`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L123)

```csharp
public static Regex Username { get; }
```

## Remarks

Pattern:

```csharp
^[a-zA-Z0-9._-]+$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [\-.0-9A-Z_a-z] atomically at least once.
○ Match if at the end of the string or if before an ending newline.
```

<a id="vin"></a>

## Vin

- Name: `Vin`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L399)

```csharp
public static Regex Vin { get; }
```

## Remarks

Pattern:

```csharp
^[A-HJ-NPR-Z0-9]{17}$
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match if at the beginning of the string.
○ Match a character in the set [0-9A-HJ-NPR-Z] exactly 17 times.
○ Match if at the end of the string or if before an ending newline.
```

<a id="whitespace"></a>

## WhiteSpace

- Name: `WhiteSpace`
- Modifiers: `static` `get`
- Returns: `Regex`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L418)

```csharp
public static Regex WhiteSpace { get; }
```

## Remarks

Pattern:

```csharp
\s+
```

Options:

```csharp
RegexOptions.CultureInvariant
```

Explanation:

```csharp
○ Match a whitespace character atomically at least once.
```
