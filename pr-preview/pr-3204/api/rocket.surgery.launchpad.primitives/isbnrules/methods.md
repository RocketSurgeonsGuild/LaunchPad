# IsbnRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Type: [IsbnRules](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/isbnrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an ISBN, an international standard book number (ISO 2108): an ISBN-13 such as `9780306406157` or an ISBN-10 such as `0306406152`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`

```csharp
public sealed class IsbnRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
