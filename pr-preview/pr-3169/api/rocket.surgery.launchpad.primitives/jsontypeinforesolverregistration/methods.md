# JsonTypeInfoResolverRegistration Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md)
- Kind: `Methods`
- Members: `8`

A JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, that the `SystemTextJsonJoint` puts in the `JsonSerializerOptions.TypeInfoResolverChain` of every `Json.JsonSerializerOptions` built from the service provider, and through them of the ASP.NET Core HTTP and MVC JSON options.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md)

```csharp
public record JsonTypeInfoResolverRegistration
{
    public JsonTypeInfoResolverRegistration <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-ijsontypeinforesolver-int"></a>

## Deconstruct(IJsonTypeInfoResolver, int)

- Name: `Deconstruct(IJsonTypeInfoResolver, int)`

```csharp
public record JsonTypeInfoResolverRegistration
{
    public void Deconstruct(
        out IJsonTypeInfoResolver Resolver,
        out int Order)
    {
        // ...
    }
}
```

## Parameters

- `Resolver` (`IJsonTypeInfoResolver`)
- `Order` (`int`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record JsonTypeInfoResolverRegistration
{
    public override bool Equals(
        object? obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object?`)
  The object to compare with the current object.

## Returns

`bool` -- `true` if the specified object is equal to the current object; otherwise, `false`.

<a id="equals-jsontypeinforesolverregistration"></a>

## Equals(JsonTypeInfoResolverRegistration?)

- Name: `Equals(JsonTypeInfoResolverRegistration?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record JsonTypeInfoResolverRegistration
{
    public bool Equals(
        JsonTypeInfoResolverRegistration? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([JsonTypeInfoResolverRegistration?](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record JsonTypeInfoResolverRegistration
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="op-equality"></a>
<a id="op-equality-jsontypeinforesolverregistration-jsontypeinforesolverregistration"></a>

## op_Equality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)

- Name: `op_Equality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record JsonTypeInfoResolverRegistration
{
    public static bool operator ==(
        JsonTypeInfoResolverRegistration? left,
        JsonTypeInfoResolverRegistration? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([JsonTypeInfoResolverRegistration?](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md))
- `right` ([JsonTypeInfoResolverRegistration?](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md))

<a id="op-inequality"></a>
<a id="op-inequality-jsontypeinforesolverregistration-jsontypeinforesolverregistration"></a>

## op_Inequality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)

- Name: `op_Inequality(JsonTypeInfoResolverRegistration?, JsonTypeInfoResolverRegistration?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record JsonTypeInfoResolverRegistration
{
    public static bool operator !=(
        JsonTypeInfoResolverRegistration? left,
        JsonTypeInfoResolverRegistration? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([JsonTypeInfoResolverRegistration?](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md))
- `right` ([JsonTypeInfoResolverRegistration?](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record JsonTypeInfoResolverRegistration
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
