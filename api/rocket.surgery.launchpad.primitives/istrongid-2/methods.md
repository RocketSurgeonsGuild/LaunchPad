# IStrongId<TSelf, TValue> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [IStrongId<TSelf, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/istrongid-2.md)
- Kind: `Methods`
- Members: `1`

A strong ID: a domain primitive that identifies an entity.

<a id="create"></a>

## Create

- Name: `Create`
- Modifiers: `static` `abstract`
- Returns: `TSelf`

Creates a new ID. Only an ID of a `Guid` can be created; any other throws `NotSupportedException`.

```csharp
public interface IStrongId<TSelf, TValue>
{
    public static abstract TSelf IStrongId<TSelf, Create()
    {
        // ...
    }
}
```

## Returns

`TSelf` -- The new ID.
