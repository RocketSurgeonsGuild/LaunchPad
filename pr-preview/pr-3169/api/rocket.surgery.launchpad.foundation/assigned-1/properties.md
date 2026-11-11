# Assigned<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Properties`
- Members: `2`

The Assigned type is used to differentiate between not set and set input values.

<a id="hasvalue"></a>

## HasValue

- Name: `HasValue`
- Modifiers: `get`
- Returns: `bool`

`true` if the Assigned was explicitly set.

```csharp
public bool HasValue { get; }
```

<a id="value"></a>

## Value

- Name: `Value`
- Modifiers: `nullable` `get`
- Returns: `T?`

The name value.

```csharp
public T? Value { get; }
```
