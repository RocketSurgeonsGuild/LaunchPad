# NotFoundException Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.primitives.md)
- Type: [NotFoundException](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.primitives/notfoundexception.md)
- Kind: `Constructors`
- Members: `3`

NotFoundException.

<a id="constructor-string"></a>

## NotFoundException(string)

- Name: `Constructor(string)`

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.primitives/notfoundexception.md) class.

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

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.primitives/notfoundexception.md) class.

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

Initializes a new instance of the [NotFoundException](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.primitives/notfoundexception.md) class.

```csharp
public class NotFoundException
{
    public NotFoundException()
    {
        // ...
    }
}
```
