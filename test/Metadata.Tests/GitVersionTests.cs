using System.Reflection;
using Rocket.Surgery.LaunchPad.Metadata;

namespace Metadata.Tests;

public class GitVersionTests(ITestContextAccessor testContext) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testContext))
{
    [Fact(Skip = "Disabled for CI")]
    public void ReturnsInformationForVersionedAssembly()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);

        version.ShouldNotBeNull();
        version.SemVer.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void EqualToEachOther()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        var version2 = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        version.ShouldNotBeNull();
        version.ShouldBe(version2);
    }

    [Fact]
    public void NotHaveAVersionForNonVersionedAssembly()
    {
        var version = GitVersion.For(typeof(string).GetTypeInfo().Assembly);
        version.HasVersion.ShouldBeFalse();
    }

    [Fact(Skip = "Disabled for CI")]
    public void ForTypeInfo()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo());

        version.ShouldNotBeNull();
        version.HasVersion.ShouldBeTrue();
    }

    [Fact(Skip = "Disabled for CI")]
    public void ForType()
    {
        var version = GitVersion.For(typeof(GitVersion));

        version.ShouldNotBeNull();
        version.HasVersion.ShouldBeTrue();
    }

    [Fact]
    public void ForManyTypesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion), typeof(GitVersion));

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Fact]
    public void ForManyTypeInfosAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Fact]
    public void ForManyAssembliesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly);

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Fact]
    public void ForManyTypesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion), typeof(GitVersion) }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Fact]
    public void ForManyTypeInfosAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo() }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Fact]
    public void ForManyAssembliesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }
}
