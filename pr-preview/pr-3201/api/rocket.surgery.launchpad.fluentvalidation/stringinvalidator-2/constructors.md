# StringInValidator<T, TProperty> Constructors

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [StringInValidator<T, TProperty>](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2.md)
- Kind: `Constructors`
- Members: `1`

A validator that is used to verify the value is in a defined list without creating a specific enum

<a id="constructor"></a>
<a id="constructor-string-bool"></a>

## StringInValidator(string[], bool)

- Name: `Constructor(string[], bool)`

A validator that is used to verify the value is in a defined list without creating a specific enum

```csharp
public class StringInValidator<T, TProperty>
{
    public StringInValidator<T, StringInValidator(string[] items, bool caseSensitive)
    {
        // ...
    }
}
```

## Parameters

- `items` (`string[]`)
  The string values to validate against
- `caseSensitive` (`bool`)
  Should character case be enforced?
