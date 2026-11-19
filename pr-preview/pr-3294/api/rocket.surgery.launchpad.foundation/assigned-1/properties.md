# Assigned<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Properties`
- Members: `2`

The Assigned type is used to differentiate between not set and set input values.

<a id="hasvalue"></a>

## HasValue

- Name: `HasValue`
- Modifiers: `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3294-merge.Sha.8ed83894fba92c2ba970626558f3059478959109/src/Foundation/Assigned.cs#L92)

`true` if the Assigned was explicitly set.

```csharp
public bool HasValue { get; }
```

<a id="value"></a>

## Value

- Name: `Value`
- Modifiers: `nullable` `get`
- Returns: `T?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3294-merge.Sha.8ed83894fba92c2ba970626558f3059478959109/src/Foundation/Assigned.cs#L87)

The name value.

```csharp
public T? Value { get; }
```
