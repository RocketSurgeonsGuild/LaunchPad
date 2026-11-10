# RequestFailedException Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives.md)
- Type: [RequestFailedException](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/requestfailedexception.md)
- Kind: `Constructors`
- Members: `3`

An exception for failed requests, with overloads to take additional details

<a id="constructor-string"></a>

## RequestFailedException(string)

- Name: `Constructor(string)`

Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.

```csharp
public class RequestFailedException
{
    public RequestFailedException(
        string message)
    {
        // ...
    }
}
```

## Parameters

- `message` (`string`)
  The message that describes the error.

<a id="constructor-string-exception"></a>

## RequestFailedException(string, Exception)

- Name: `Constructor(string, Exception)`

Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.

```csharp
public class RequestFailedException
{
    public RequestFailedException(
        string message,
        Exception innerException)
    {
        // ...
    }
}
```

## Parameters

- `message` (`string`)
  The error message that explains the reason for the exception.
- `innerException` (`Exception`)
  The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.

<a id="constructor"></a>

## RequestFailedException

- Name: `Constructor`

Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.

```csharp
public class RequestFailedException
{
    public RequestFailedException()
    {
        // ...
    }
}
```
