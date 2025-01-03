using System.Reflection;
using Rocket.Surgery.LaunchPad.Metadata;

namespace Metadata.Tests;

public class GitVersionTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper))
{
    [Fact(Skip = "Disabled for CI")]
    public void ReturnsInformationForVersionedAssembly()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);

        version.Should().NotBeNull();
        version.SemVer.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void EqualToEachOther()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        var version2 = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        version.Should().NotBeNull();
        version.Should().Be(version2);
    }

    [Fact]
    public void NotHaveAVersionForNonVersionedAssembly()
    {
        var version = GitVersion.For(typeof(string).GetTypeInfo().Assembly);
        version.HasVersion.Should().BeFalse();
    }

    [Fact(Skip = "Disabled for CI")]
    public void ForTypeInfo()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo());

        version.Should().NotBeNull();
        version.HasVersion.Should().BeTrue();
    }

    [Fact(Skip = "Disabled for CI")]
    public void ForType()
    {
        var version = GitVersion.For(typeof(GitVersion));

        version.Should().NotBeNull();
        version.HasVersion.Should().BeTrue();
    }

    [Fact]
    public void ForManyTypesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion), typeof(GitVersion));

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }

    [Fact]
    public void ForManyTypeInfosAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo());

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }

    [Fact]
    public void ForManyAssembliesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly);

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }

    [Fact]
    public void ForManyTypesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion), typeof(GitVersion) }.AsEnumerable());

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }

    [Fact]
    public void ForManyTypeInfosAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo() }.AsEnumerable());

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }

    [Fact]
    public void ForManyAssembliesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly }.AsEnumerable());

        version.Should().NotBeNull();
        version.Should().HaveCount(1);
    }
}
