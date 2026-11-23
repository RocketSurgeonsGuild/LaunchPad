# IDomainPrimitive<TSelf, TValue> Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [IDomainPrimitive<TSelf, TValue>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-2.md)
- Kind: `Properties`
- Members: `2`

A domain primitive that holds one value.

<a id="description"></a>

## Description

- Name: `Description`
- Modifiers: `static` `abstract` `get`
- Returns: `DomainPrimitiveDescription<TSelf, TValue>`

What an integration needs to know about the domain primitive: its value and the constraints of its rules.

```csharp
public static abstract DomainPrimitiveDescription<TSelf, TValue> IDomainPrimitive<TSelf, Description { get; }
```

<a id="value"></a>

## Value

- Name: `Value`
- Modifiers: `abstract` `get`
- Returns: `TValue`

The value.

```csharp
public abstract TValue IDomainPrimitive<TSelf, Value { get; }
```
