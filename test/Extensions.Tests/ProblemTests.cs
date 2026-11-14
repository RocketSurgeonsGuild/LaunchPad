using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Extensions.Tests;

public class ProblemTests
{
    public static IEnumerable<Func<(ProblemDetailsException Exception, string Message)>> DefaultExceptions()
    {
        yield return () => (new NotFoundException(), "Not Found");
        yield return () => (new NotAuthorizedException(), "Not Authorized");
        yield return () => (new RequestFailedException(), "");
    }

    [Test]
    [MethodDataSource(nameof(DefaultExceptions))]
    public async Task Should_Give_Each_Exception_A_Default_Message_And_No_Details(ProblemDetailsException exception, string message)
    {
        await exception.Message.Should().BeEqualTo(message);
        await exception.Title.Should().BeNull();
        await exception.Link.Should().BeNull();
        await exception.Instance.Should().BeNull();
        await exception.Properties.Should().BeEmpty();
    }

    public static IEnumerable<Func<(Func<string, Exception, ProblemDetailsException> Create, Type Type)>> ExceptionsWithInnerExceptions()
    {
        yield return () => ((message, inner) => new NotFoundException(message, inner), typeof(NotFoundException));
        yield return () => ((message, inner) => new NotAuthorizedException(message, inner), typeof(NotAuthorizedException));
        yield return () => ((message, inner) => new RequestFailedException(message, inner), typeof(RequestFailedException));
    }

    [Test]
    [MethodDataSource(nameof(ExceptionsWithInnerExceptions))]
    public async Task Should_Keep_The_Inner_Exception(Func<string, Exception, ProblemDetailsException> create, Type type)
    {
        var inner = new InvalidOperationException("inner");

        var exception = create("outer", inner);

        await exception.Should().BeOfType(type);
        await exception.Message.Should().BeEqualTo("outer");
        await exception.InnerException.Should().BeSameReferenceAs(inner);
    }

    [Test]
    public async Task Should_Carry_The_Problem_Details()
    {
        IProblemDetailsData data = new RequestFailedException("bad request")
        {
            Title = "Bad request",
            Link = "https://example.com/problems/bad-request",
            Instance = "/rockets",
            Properties = { ["field"] = "name", },
        };

        await data.Title.Should().BeEqualTo("Bad request");
        await data.Link.Should().BeEqualTo("https://example.com/problems/bad-request");
        await data.Instance.Should().BeEqualTo("/rockets");
        await data.Properties.Should().ContainKeyWithValue("field", "name");
    }

    [Test]
    public async Task Should_Configure_And_Validate_An_Existing_Options_Instance()
    {
        using var provider = new ServiceCollection()
                            .Configure<LaunchOptions>(options => options.Name += "-configured")
                            .PostConfigure<LaunchOptions>(options => options.Name += "-post")
                            .BuildServiceProvider();
        var options = new LaunchOptions { Name = "existing", };

        ExistingValueOptions.Apply(provider, options, Options.DefaultName);

        await options.Name.Should().BeEqualTo("existing-configured-post");
    }

    [Test]
    public async Task Should_Fail_When_The_Existing_Options_Instance_Is_Invalid()
    {
        using var provider = new ServiceCollection()
                            .AddSingleton<IValidateOptions<LaunchOptions>>(
                                 new ValidateOptions<LaunchOptions>(Options.DefaultName, options => options.Name.Length > 0, "a name is required")
                             )
                            .BuildServiceProvider();

        await ( await ( (Action)( () => ExistingValueOptions.Apply(provider, new LaunchOptions(), Options.DefaultName) ) ).Should().Throw<OptionsValidationException>() )!
           .Failures.Should().BeEquivalentTo(["a name is required"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Enumerate_An_Enumerator()
    {
        using var enumerator = new List<int> { 1, 2, 3, }.GetEnumerator() as IEnumerator<int>;
        var values = new List<int>();

        foreach (var value in enumerator)
        {
            values.Add(value);
        }

        await values.Should().BeEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
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

        await values.Should().BeEquivalentTo([1, 2], CollectionOrdering.Matching);

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
