# IDomainPrimitive<TSelf> Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Type: [IDomainPrimitive<TSelf>](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)
- Kind: `Properties`
- Members: `2`

A domain primitive: a type the Primitives generator completes, which only holds values its rules accept.

<a id="isdefault"></a>

## IsDefault

- Name: `IsDefault`
- Modifiers: `virtual` `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/IDomainPrimitive.cs#L22)

Whether this is the default value of a domain primitive declared as a struct, which holds no value. A domain primitive declared as a class is never default: a missing value is `null`.

```csharp
public virtual bool IsDefault { get; }
```

<a id="primitivename"></a>

## PrimitiveName

- Name: `PrimitiveName`
- Modifiers: `static` `abstract` `get`
- Returns: `string`

The name of the domain primitive, used in errors and diagnostics.

```csharp
public static abstract string PrimitiveName { get; }
```
