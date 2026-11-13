# NotFoundException Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation.md)
- Type: [NotFoundException](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/notfoundexception.md)
- Kind: `Constructors`
- Members: `3`

NotFoundException.

<a id="constructor-string"></a>

## NotFoundException(string)

- Name: `Constructor(string)`

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/notfoundexception.md) class.

```csharp
public class NotFoundException
{
    public NotFoundException(
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

## NotFoundException(string, Exception)

- Name: `Constructor(string, Exception)`

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/notfoundexception.md) class.

```csharp
public class NotFoundException
{
    public NotFoundException(
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

## NotFoundException

- Name: `Constructor`

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/notfoundexception.md) class.

```csharp
public class NotFoundException
{
    public NotFoundException()
    {
        // ...
    }
}
```
