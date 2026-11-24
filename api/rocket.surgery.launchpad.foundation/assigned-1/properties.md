# Assigned<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Properties`
- Members: `2`

The Assigned type is used to differentiate between not set and set input values.

<a id="hasvalue"></a>

## HasValue

- Name: `HasValue`
- Modifiers: `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.4a5cb286621c756019f759ab46381031aa177050/src/Foundation/Assigned.cs#L92)

`true` if the Assigned was explicitly set.

```csharp
public bool HasValue { get; }
```

<a id="value"></a>

## Value

- Name: `Value`
- Modifiers: `nullable` `get`
- Returns: `T?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.4a5cb286621c756019f759ab46381031aa177050/src/Foundation/Assigned.cs#L87)

The name value.

```csharp
public T? Value { get; }
```
