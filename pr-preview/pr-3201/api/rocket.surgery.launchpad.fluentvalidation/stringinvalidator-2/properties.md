# StringInValidator<T, TProperty> Properties

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [StringInValidator<T, TProperty>](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2.md)
- Kind: `Properties`
- Members: `2`

A validator that is used to verify the value is in a defined list without creating a specific enum

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `override` `get`
- Returns: `string`

The name of the validator. This is usually the type name without any generic parameters. This is used as the default Error Code for the validator.

```csharp
public override string StringInValidator<T, Name { get; }
```

<a id="values"></a>

## Values

- Name: `Values`
- Modifiers: `get`
- Returns: `IEnumerable<string>`

The values that being enforced.

```csharp
public IEnumerable<string> StringInValidator<T, Values { get; }
```
