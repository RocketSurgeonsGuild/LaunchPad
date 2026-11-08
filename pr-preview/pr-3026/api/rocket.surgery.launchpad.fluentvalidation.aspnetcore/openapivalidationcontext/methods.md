# OpenApiValidationContext Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [OpenApiValidationContext](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md)
- Kind: `Methods`
- Members: `8`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `virtual`
- Returns: [OpenApiValidationContext](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md)

```csharp
public record OpenApiValidationContext
{
    public virtual OpenApiValidationContext <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-openapischema-openapischema-openapischematransformercontext-ipropertyvalidator-string-irulecomponent"></a>

## Deconstruct(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)

- Name: `Deconstruct(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)`

```csharp
public record OpenApiValidationContext
{
    public void Deconstruct(
        out OpenApiSchema TypeSchema,
        out OpenApiSchema PropertySchema,
        out OpenApiSchemaTransformerContext TransformerContext,
        out IPropertyValidator PropertyValidator,
        out string PropertyKey,
        out IRuleComponent RuleComponent)
    {
        // ...
    }
}
```

## Parameters

- `TypeSchema` (`OpenApiSchema`)
- `PropertySchema` (`OpenApiSchema`)
- `TransformerContext` (`OpenApiSchemaTransformerContext`)
- `PropertyValidator` (`IPropertyValidator`)
- `PropertyKey` (`string`)
- `RuleComponent` (`IRuleComponent`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record OpenApiValidationContext
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

<a id="equals-openapivalidationcontext"></a>

## Equals(OpenApiValidationContext?)

- Name: `Equals(OpenApiValidationContext?)`
- Modifiers: `virtual`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record OpenApiValidationContext
{
    public virtual bool Equals(
        OpenApiValidationContext? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([OpenApiValidationContext?](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))
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
public record OpenApiValidationContext
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
<a id="op-equality-openapivalidationcontext-openapivalidationcontext"></a>

## op_Equality(OpenApiValidationContext?, OpenApiValidationContext?)

- Name: `op_Equality(OpenApiValidationContext?, OpenApiValidationContext?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record OpenApiValidationContext
{
    public static bool operator ==(
        OpenApiValidationContext? left,
        OpenApiValidationContext? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([OpenApiValidationContext?](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))
- `right` ([OpenApiValidationContext?](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))

<a id="op-inequality"></a>
<a id="op-inequality-openapivalidationcontext-openapivalidationcontext"></a>

## op_Inequality(OpenApiValidationContext?, OpenApiValidationContext?)

- Name: `op_Inequality(OpenApiValidationContext?, OpenApiValidationContext?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record OpenApiValidationContext
{
    public static bool operator !=(
        OpenApiValidationContext? left,
        OpenApiValidationContext? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([OpenApiValidationContext?](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))
- `right` ([OpenApiValidationContext?](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record OpenApiValidationContext
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
