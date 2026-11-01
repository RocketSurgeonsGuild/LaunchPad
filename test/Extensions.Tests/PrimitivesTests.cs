using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Extensions;

namespace Extensions.Tests;

public class PrimitivesTests
{
    public static IEnumerable<Func<(ProblemDetailsException Exception, string Message)>> DefaultExceptions()
    {
        yield return () => (new NotFoundException(), "Not Found");
        yield return () => (new NotAuthorizedException(), "Not Authorized");
        yield return () => (new RequestFailedException(), "");
    }

    [Test]
    [MethodDataSource(nameof(DefaultExceptions))]
    public void Should_Give_Each_Exception_A_Default_Message_And_No_Details(ProblemDetailsException exception, string message)
    {
        exception.Message.ShouldBe(message);
        exception.Title.ShouldBeNull();
        exception.Link.ShouldBeNull();
        exception.Instance.ShouldBeNull();
        exception.Properties.ShouldBeEmpty();
    }

    public static IEnumerable<Func<(Func<string, Exception, ProblemDetailsException> Create, Type Type)>> ExceptionsWithInnerExceptions()
    {
        yield return () => ((message, inner) => new NotFoundException(message, inner), typeof(NotFoundException));
        yield return () => ((message, inner) => new NotAuthorizedException(message, inner), typeof(NotAuthorizedException));
        yield return () => ((message, inner) => new RequestFailedException(message, inner), typeof(RequestFailedException));
    }

    [Test]
    [MethodDataSource(nameof(ExceptionsWithInnerExceptions))]
    public void Should_Keep_The_Inner_Exception(Func<string, Exception, ProblemDetailsException> create, Type type)
    {
        var inner = new InvalidOperationException("inner");

        var exception = create("outer", inner);

        exception.ShouldBeOfType(type);
        exception.Message.ShouldBe("outer");
        exception.InnerException.ShouldBeSameAs(inner);
    }

    [Test]
    public void Should_Carry_The_Problem_Details()
    {
        IProblemDetailsData data = new RequestFailedException("bad request")
        {
            Title = "Bad request",
            Link = "https://example.com/problems/bad-request",
            Instance = "/rockets",
            Properties = { ["field"] = "name", },
        };

        data.Title.ShouldBe("Bad request");
        data.Link.ShouldBe("https://example.com/problems/bad-request");
        data.Instance.ShouldBe("/rockets");
        data.Properties.ShouldContainKeyAndValue("field", "name");
    }

    [Test]
    public void Should_Configure_And_Validate_An_Existing_Options_Instance()
    {
        using var provider = new ServiceCollection()
                            .Configure<LaunchOptions>(options => options.Name += "-configured")
                            .PostConfigure<LaunchOptions>(options => options.Name += "-post")
                            .BuildServiceProvider();
        var options = new LaunchOptions { Name = "existing", };

        ExistingValueOptions.Apply(provider, options, Options.DefaultName);

        options.Name.ShouldBe("existing-configured-post");
    }

    [Test]
    public void Should_Fail_When_The_Existing_Options_Instance_Is_Invalid()
    {
        using var provider = new ServiceCollection()
                            .AddSingleton<IValidateOptions<LaunchOptions>>(
                                 new ValidateOptions<LaunchOptions>(Options.DefaultName, options => options.Name.Length > 0, "a name is required")
                             )
                            .BuildServiceProvider();

        Should
           .Throw<OptionsValidationException>(() => ExistingValueOptions.Apply(provider, new LaunchOptions(), Options.DefaultName))
           .Failures.ShouldBe(["a name is required"]);
    }

    [Test]
    public void Should_Enumerate_An_Enumerator()
    {
        using var enumerator = new List<int> { 1, 2, 3, }.GetEnumerator() as IEnumerator<int>;
        var values = new List<int>();

        foreach (var value in enumerator)
        {
            values.Add(value);
        }

        values.ShouldBe([1, 2, 3]);
    }

    [Test]
    public async Task Should_Enumerate_An_Async_Enumerator()
    {
        await using var enumerator = Values().GetAsyncEnumerator();
        var values = new List<int>();

        await foreach (var value in enumerator)
        {
            values.Add(value);
        }

        values.ShouldBe([1, 2]);

        static async IAsyncEnumerable<int> Values()
        {
            yield return 1;
            await Task.Yield();
            yield return 2;
        }
    }

    private sealed class LaunchOptions
    {
        public string Name { get; set; } = "";
    }
}
