using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests;

public class AssignedTests
{
    [Test]
    public async Task Should_Know_A_Set_Value_From_An_Unset_One()
    {
        Assigned<string> set = "launch";
        var unset = Assigned<string>.Empty("fallback");

        await set.HasValue.Should().BeTrue();
        await set.Value.Should().BeEqualTo("launch");
        await unset.HasValue.Should().BeFalse();
        await unset.Value.Should().BeEqualTo("fallback");
    }

    [Test]
    public async Task Should_Treat_A_Null_Set_Explicitly_As_A_Value()
    {
        var assigned = new Assigned<string>(null);

        await assigned.HasValue.Should().BeTrue();
        await assigned.ToString().Should().BeEqualTo("null");
    }

    [Test]
    public async Task Should_Describe_Itself()
    {
        await new Assigned<int>(3).ToString().Should().BeEqualTo("3");
        await Assigned<int>.Empty().ToString().Should().BeEqualTo("unspecified");
    }

    [Test]
    public async Task Should_Convert_Back_To_Its_Value()
    {
        string? value = new Assigned<string>("launch");

        await value.Should().BeEqualTo("launch");
    }

    [Test]
    public async Task Should_Be_Equal_When_Both_Are_Set_To_The_Same_Value()
    {
        var left = new Assigned<int>(3);
        var right = new Assigned<int>(3);

        await left.Equals(right).Should().BeTrue();
        await ( left == right ).Should().BeTrue();
        await ( left != right ).Should().BeFalse();
        await left.Equals((object)right).Should().BeTrue();
        await left.GetHashCode().Should().BeEqualTo(right.GetHashCode());
    }

    [Test]
    public async Task Should_Not_Be_Equal_When_The_Values_Differ()
    {
        await ( new Assigned<int>(3) == new Assigned<int>(4) ).Should().BeFalse();
        await ( new Assigned<int>(3) != new Assigned<int>(4) ).Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_Equal_When_Only_One_Is_Set()
    {
        // an unset value with the same default is still not a set one
        await new Assigned<int>(0).Equals(Assigned<int>.Empty()).Should().BeFalse();
        await Assigned<int>.Empty().Equals(new Assigned<int>(0)).Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Equal_When_Neither_Is_Set_Whatever_The_Defaults()
    {
        await Assigned<int>.Empty(1).Equals(Assigned<int>.Empty(2)).Should().BeTrue();
        await Assigned<int>.Empty(1).GetHashCode().Should().BeEqualTo(0);
    }

    [Test]
    public async Task Should_Equal_Null_Only_When_Unset()
    {
        await Assigned<string>.Empty().Equals(null).Should().BeTrue();
        await Assigned<string>.Empty().Equals((object?)null).Should().BeTrue();
        await new Assigned<string>("launch").Equals(null).Should().BeFalse();
        await new Assigned<string>("launch").Equals((object?)null).Should().BeFalse();
    }

    [Test]
    public async Task Should_Not_Equal_Another_Type() => await new Assigned<int>(3).Equals((object)3).Should().BeFalse();
}
