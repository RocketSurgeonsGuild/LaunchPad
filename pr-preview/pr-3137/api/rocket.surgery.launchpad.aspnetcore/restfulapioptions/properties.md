# RestfulApiOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [RestfulApiOptions](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions.md)
- Kind: `Properties`
- Members: `4`

Options used for Restful Api Options.

<a id="builders"></a>

## Builders

- Name: `Builders`
- Modifiers: `get`
- Returns: `Collection<RestfulApiMethodBuilder>`

The related method builders that are used to identify methods by name, prefix, type, etc.

```csharp
public Collection<RestfulApiMethodBuilder> Builders { get; }
```

<a id="methodstatuscodemap"></a>

## MethodStatusCodeMap

- Name: `MethodStatusCodeMap`
- Modifiers: `get`
- Returns: `IDictionary<RestfulApiMethod, int>`

The cache of default status codes for a given method type.

```csharp
public IDictionary<RestfulApiMethod, int> MethodStatusCodeMap { get; }
```

<a id="validationproblemdetailstype"></a>

## ValidationProblemDetailsType

- Name: `ValidationProblemDetailsType`
- Modifiers: `get; set`
- Returns: `Type`

The response type documented for the [RestfulApiOptions.ValidationStatusCode](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#validationstatuscode). A validation integration (such as Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore) replaces it with its own problem details.

```csharp
public Type ValidationProblemDetailsType { get; set; }
```

<a id="validationstatuscode"></a>

## ValidationStatusCode

- Name: `ValidationStatusCode`
- Modifiers: `get; set`
- Returns: `int`

The factory to use for Validation results

```csharp
public int ValidationStatusCode { get; set; }
```
