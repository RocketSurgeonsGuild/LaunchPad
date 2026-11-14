# FluentValidationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.fluentvalidation.md)
- Version: `2.0.1-pr.1196`
- Namespace: `FluentValidation`
- Target framework: `net10.0`

Fluent validations

## Definition

```csharp
namespace FluentValidation;

public static class FluentValidationExtensions
{
    // ...
}
```

## Methods

- [GetValidator(IServiceProvider)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationextensions/methods.md#getvalidator-iserviceprovider) : `IValidator<T>` `extension` -- Get a fluent validation validator if defined.
- [GetValidator(IServiceProvider, Type?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationextensions/methods.md#getvalidator-iserviceprovider-type) : `IValidator?` `extension` -- Get a fluent validation validator if defined.
- [IsOneOf(IRuleBuilder<T, TProperty?>, bool, string[])](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationextensions/methods.md#isoneof-irulebuilder-t-tproperty-bool-string) : `IRuleBuilderOptions<T, TProperty?>` `extension` -- Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.
- [IsOneOf(IRuleBuilder<T, TProperty>, string[])](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationextensions/methods.md#isoneof-irulebuilder-t-tproperty-string) : `IRuleBuilderOptions<T, TProperty>` `extension` -- Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.
