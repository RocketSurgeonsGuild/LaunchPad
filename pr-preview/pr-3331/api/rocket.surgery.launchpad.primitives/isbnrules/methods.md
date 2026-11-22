# IsbnRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives.md)
- Type: [IsbnRules](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/isbnrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an ISBN, an international standard book number (ISO 2108): an ISBN-13 such as `9780306406157` or an ISBN-10 such as `0306406152`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Primitives/StrongTypes/Isbn.cs#L28-L31)

```csharp
public sealed class IsbnRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
