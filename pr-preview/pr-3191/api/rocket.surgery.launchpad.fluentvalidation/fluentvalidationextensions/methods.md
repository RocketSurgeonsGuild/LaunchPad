# FluentValidationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [FluentValidationExtensions](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationextensions.md)
- Kind: `Methods`
- Members: `4`

Fluent validations

<a id="getvalidator"></a>
<a id="getvalidator-iserviceprovider"></a>

## GetValidator(IServiceProvider)

- Name: `GetValidator(IServiceProvider)`
- Modifiers: `extension` `nullable`
- Returns: `IValidator<T>`

Get a fluent validation validator if defined.

```csharp
public static class FluentValidationExtensions
{
    public static IValidator<T>? GetValidator<T>(
        this IServiceProvider serviceProvider)
    {
        // ...
    }
}
```

## Parameters

- `serviceProvider` (`IServiceProvider`)

<a id="getvalidator-iserviceprovider-type"></a>

## GetValidator(IServiceProvider, Type?)

- Name: `GetValidator(IServiceProvider, Type?)`
- Modifiers: `extension` `nullable`
- Returns: `IValidator?`

Get a fluent validation validator if defined.

```csharp
public static class FluentValidationExtensions
{
    public static IValidator? GetValidator(
        this IServiceProvider serviceProvider,
        Type? type)
    {
        // ...
    }
}
```

## Parameters

- `serviceProvider` (`IServiceProvider`)
- `type` (`Type?`)

<a id="isoneof"></a>
<a id="isoneof-irulebuilder-t-tproperty-bool-string"></a>

## IsOneOf(IRuleBuilder<T, TProperty?>, bool, string[])

- Name: `IsOneOf(IRuleBuilder<T, TProperty?>, bool, string[])`
- Modifiers: `extension`
- Returns: `IRuleBuilderOptions<T, TProperty?>`

Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.

```csharp
public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty?> IsOneOf<T, TProperty>(
        this IRuleBuilder<T, TProperty?> ruleBuilder,
        bool caseSensitive,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `ruleBuilder` (`IRuleBuilder<T, TProperty?>`)
  The rule builder on which the validator should be defined
- `caseSensitive` (`bool`)
  If the comparison between the string and the enum names should be case sensitive
- `values` (`string[]`)
  The values to match against

<a id="isoneof-irulebuilder-t-tproperty-string"></a>

## IsOneOf(IRuleBuilder<T, TProperty>, string[])

- Name: `IsOneOf(IRuleBuilder<T, TProperty>, string[])`
- Modifiers: `extension`
- Returns: `IRuleBuilderOptions<T, TProperty>`

Defines a validator on the current rule builder that ensures that the specific value is one of the values given in the list.

```csharp
public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> IsOneOf<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `ruleBuilder` (`IRuleBuilder<T, TProperty>`)
  The rule builder on which the validator should be defined
- `values` (`string[]`)
  The values to match against
