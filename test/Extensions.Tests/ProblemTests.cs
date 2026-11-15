using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Extensions;

namespace Extensions.Tests;

public class ProblemTests
{
    [Test]
    public async Task Should_Carry_The_Problem_Details()
    {
        IProblemDetailsData data = new Conflict("already launched")
        {
            Title = "Conflict",
            Link = "https://example.com/problems/conflict",
            Instance = "/rockets",
            Properties = { ["field"] = "name", },
        };

        await data.Title.Should().BeEqualTo("Conflict");
        await data.Link.Should().BeEqualTo("https://example.com/problems/conflict");
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
