# IPropertyTracking<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.foundation.md)
- Type: [IPropertyTracking<T>](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.foundation/ipropertytracking-1.md)
- Kind: `Methods`
- Members: `2`

Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties

<a id="applychanges"></a>
<a id="applychanges-t"></a>

## ApplyChanges(T)

- Name: `ApplyChanges(T)`
- Modifiers: `abstract`
- Returns: `T`

Method used apply changes from a given property tracking class.

```csharp
public interface IPropertyTracking<T>
{
    public abstract T ApplyChanges(
        T state)
    {
        // ...
    }
}
```

## Parameters

- `state` (`T`)

## Remarks

For records this will return a new record instance for classes it will mutate the existing instance.

<a id="resetchanges"></a>

## ResetChanges

- Name: `ResetChanges`
- Modifiers: `abstract`

Reset the state of the property tracking to only track future changes

```csharp
public interface IPropertyTracking<T>
{
    public abstract void ResetChanges()
    {
        // ...
    }
}
```
