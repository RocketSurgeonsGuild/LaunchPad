# GraphQLAuthenticatedAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [GraphQLAuthenticatedAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlauthenticatedattribute.md)
- Kind: `Constructors`
- Members: `1`

States that a field requires an authenticated caller with `@authenticated`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare no scopes.

<a id="constructor"></a>

## GraphQLAuthenticatedAttribute

- Name: `Constructor`

```csharp
public sealed class GraphQLAuthenticatedAttribute
{
    public GraphQLAuthenticatedAttribute()
    {
        // ...
    }
}
```
