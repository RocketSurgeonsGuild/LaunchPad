using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests;

public class AssignedTests
{
    [Test]
    public void Should_Know_A_Set_Value_From_An_Unset_One()
    {
        Assigned<string> set = "launch";
        var unset = Assigned<string>.Empty("fallback");

        set.HasValue.ShouldBeTrue();
        set.Value.ShouldBe("launch");
        unset.HasValue.ShouldBeFalse();
        unset.Value.ShouldBe("fallback");
    }

    [Test]
    public void Should_Treat_A_Null_Set_Explicitly_As_A_Value()
    {
        var assigned = new Assigned<string>(null);

        assigned.HasValue.ShouldBeTrue();
        assigned.ToString().ShouldBe("null");
    }

    [Test]
    public void Should_Describe_Itself()
    {
        new Assigned<int>(3).ToString().ShouldBe("3");
        Assigned<int>.Empty().ToString().ShouldBe("unspecified");
    }

    [Test]
    public void Should_Convert_Back_To_Its_Value()
    {
        string? value = new Assigned<string>("launch");

        value.ShouldBe("launch");
    }

    [Test]
    public void Should_Be_Equal_When_Both_Are_Set_To_The_Same_Value()
    {
        var left = new Assigned<int>(3);
        var right = new Assigned<int>(3);

        left.Equals(right).ShouldBeTrue();
        ( left == right ).ShouldBeTrue();
        ( left != right ).ShouldBeFalse();
        left.Equals((object)right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Test]
    public void Should_Not_Be_Equal_When_The_Values_Differ()
    {
        ( new Assigned<int>(3) == new Assigned<int>(4) ).ShouldBeFalse();
        ( new Assigned<int>(3) != new Assigned<int>(4) ).ShouldBeTrue();
    }

    [Test]
    public void Should_Not_Be_Equal_When_Only_One_Is_Set()
    {
        // an unset value with the same default is still not a set one
        new Assigned<int>(0).Equals(Assigned<int>.Empty()).ShouldBeFalse();
        Assigned<int>.Empty().Equals(new Assigned<int>(0)).ShouldBeFalse();
    }

    [Test]
    public void Should_Be_Equal_When_Neither_Is_Set_Whatever_The_Defaults()
    {
        Assigned<int>.Empty(1).Equals(Assigned<int>.Empty(2)).ShouldBeTrue();
        Assigned<int>.Empty(1).GetHashCode().ShouldBe(0);
    }

    [Test]
    public void Should_Equal_Null_Only_When_Unset()
    {
        Assigned<string>.Empty().Equals(null).ShouldBeTrue();
        Assigned<string>.Empty().Equals((object?)null).ShouldBeTrue();
        new Assigned<string>("launch").Equals(null).ShouldBeFalse();
        new Assigned<string>("launch").Equals((object?)null).ShouldBeFalse();
    }

    [Test]
    public void Should_Not_Equal_Another_Type() => new Assigned<int>(3).Equals((object)3).ShouldBeFalse();
}
