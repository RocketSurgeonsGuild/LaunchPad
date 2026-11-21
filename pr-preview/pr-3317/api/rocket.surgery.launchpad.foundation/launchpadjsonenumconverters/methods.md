# LaunchPadJsonEnumConverters Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md)
- Kind: `Methods`
- Members: `2`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="addenumconverters"></a>
<a id="addenumconverters-jsonserializeroptions"></a>

## AddEnumConverters(JsonSerializerOptions)

- Name: `AddEnumConverters(JsonSerializerOptions)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/Foundation/LaunchPadJsonEnumConverters.cs#L54-L60)

Adds every registered converter the options don't have yet.

```csharp
public static class LaunchPadJsonEnumConverters
{
    public static JsonSerializerOptions AddEnumConverters(
        this JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `options` (`JsonSerializerOptions`)
  The options.

## Returns

`JsonSerializerOptions` -- The options.

<a id="register"></a>
<a id="register-jsonstringenumconverter-tenum"></a>

## Register(JsonStringEnumConverter<TEnum>)

- Name: `Register(JsonStringEnumConverter<TEnum>)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/Foundation/LaunchPadJsonEnumConverters.cs#L26-L31)

Registers an enum converter. A later registration for the same type replaces the earlier one.

```csharp
public static class LaunchPadJsonEnumConverters
{
    public static void Register<TEnum>(
        JsonStringEnumConverter<TEnum> converter)
    {
        // ...
    }
}
```

## Parameters

- `converter` (`JsonStringEnumConverter<TEnum>`)
  The converter, usually a `Serialization.JsonStringEnumConverter`1`.
