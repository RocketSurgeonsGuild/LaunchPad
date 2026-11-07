# RestfulApiMethodBuilder Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)
- Kind: `Methods`
- Members: `9`

This class allows you to define methods default conventions when using the [RestfulApiController](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapicontroller.md) This allows the response codes to be automatically inferred based on the rules defined.

<a id="hasparameter"></a>
<a id="hasparameter-index"></a>

## HasParameter(Index)

- Name: `HasParameter(Index)`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match only if a parameter exists at the given index

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder HasParameter(
        Index parameter)
    {
        // ...
    }
}
```

## Parameters

- `parameter` (`Index`)
  The parameter, maybe a positive or negative `Index`

## Remarks

The index can be either positive or negative to allow for comparing against first or last parameters

<a id="matchname"></a>
<a id="matchname-string-string"></a>

## MatchName(string, string[])

- Name: `MatchName(string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match based one of the given method names

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchName(
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
  The first name
- `values` (`string[]`)
  Any additional names

<a id="matchparametercount"></a>
<a id="matchparametercount-int"></a>

## MatchParameterCount(int)

- Name: `MatchParameterCount(int)`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match only if there are a given number of parameters

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchParameterCount(
        int count)
    {
        // ...
    }
}
```

## Parameters

- `count` (`int`)

<a id="matchparametername"></a>
<a id="matchparametername-index-string-string"></a>

## MatchParameterName(Index, string, string[])

- Name: `MatchParameterName(Index, string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Matched on the names of the parameter at the given index

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchParameterName(
        Index parameter,
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `parameter` (`Index`)
  The parameter, maybe a positive or negative `Index`
- `value` (`string`)
  The first name
- `values` (`string[]`)
  Any additional names

## Remarks

The index can be either positive or negative to allow for comparing against first or last parameters

<a id="matchparameterprefix"></a>
<a id="matchparameterprefix-index-string-string"></a>

## MatchParameterPrefix(Index, string, string[])

- Name: `MatchParameterPrefix(Index, string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match against the prefixes for the given parameter at the given index

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchParameterPrefix(
        Index parameter,
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `parameter` (`Index`)
  The parameter, maybe a positive or negative `Index`
- `value` (`string`)
  The first prefix
- `values` (`string[]`)
  Any additional prefixes

## Remarks

The index can be either positive or negative to allow for comparing against first or last parameters

<a id="matchparametersuffix"></a>
<a id="matchparametersuffix-index-string-string"></a>

## MatchParameterSuffix(Index, string, string[])

- Name: `MatchParameterSuffix(Index, string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match against the suffixes for the given parameter at the given index

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchParameterSuffix(
        Index parameter,
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `parameter` (`Index`)
  The parameter, maybe a positive or negative `Index`
- `value` (`string`)
  The first suffix
- `values` (`string[]`)
  Any additional suffixes

## Remarks

The index can be either positive or negative to allow for comparing against first or last parameters

<a id="matchparametertype"></a>
<a id="matchparametertype-index-type"></a>

## MatchParameterType(Index, Type)

- Name: `MatchParameterType(Index, Type)`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match the type parameter at the given index.

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchParameterType(
        Index parameter,
        Type type)
    {
        // ...
    }
}
```

## Parameters

- `parameter` (`Index`)
  The parameter, maybe a positive or negative `Index`
- `type` (`Type`)
  The type of the paramter

## Remarks

The index can be either positive or negative to allow for comparing against first or last parameters

<a id="matchprefix"></a>
<a id="matchprefix-string-string"></a>

## MatchPrefix(string, string[])

- Name: `MatchPrefix(string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match against one of the given prefixes

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchPrefix(
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
  The first prefix
- `values` (`string[]`)
  Any additional prefixes

<a id="matchsuffix"></a>
<a id="matchsuffix-string-string"></a>

## MatchSuffix(string, string[])

- Name: `MatchSuffix(string, string[])`
- Returns: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)

Match against one of the given suffixes

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder MatchSuffix(
        string value,
        params string[] values)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
  The first suffix
- `values` (`string[]`)
  Any additional suffixes
