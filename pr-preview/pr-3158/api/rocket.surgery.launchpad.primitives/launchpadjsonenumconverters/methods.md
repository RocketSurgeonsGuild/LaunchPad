# LaunchPadJsonEnumConverters Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/launchpadjsonenumconverters.md)
- Kind: `Methods`
- Members: `2`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="addenumconverters"></a>
<a id="addenumconverters-jsonserializeroptions"></a>

## AddEnumConverters(JsonSerializerOptions)

- Name: `AddEnumConverters(JsonSerializerOptions)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`

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
