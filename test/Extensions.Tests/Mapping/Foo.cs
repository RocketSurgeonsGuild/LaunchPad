#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Extensions.Tests.Mapping;

public static class Foo
{
    public static Foo<T> Create<T>(T value)
    {
        return new Foo<T> { Value = value };
    }
}

public class Foo<T>
{
    public T Value { get; set; }
}
