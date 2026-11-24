# PrimitiveError Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitiveError](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Kind: `Constructors`
- Members: `1`

Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## PrimitiveError(string?, string?)

- Name: `Constructor(string?, string?)`

Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

```csharp
public struct PrimitiveError
{
    public PrimitiveError(
        string? Code,
        string? Message)
    {
        // ...
    }
}
```

## Parameters

- `Code` (`string?`)
  The code, or `null` when there is no error.
- `Message` (`string?`)
  The message.
