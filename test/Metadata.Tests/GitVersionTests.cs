using System.Reflection;
using Rocket.Surgery.LaunchPad.Metadata;

namespace Metadata.Tests;

public class GitVersionTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    [Skip("Disabled for CI")]
    public void ReturnsInformationForVersionedAssembly()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);

        version.ShouldNotBeNull();
        version.SemVer.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public void EqualToEachOther()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        var version2 = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly);
        version.ShouldNotBeNull();
        version.ShouldBe(version2);
    }

    [Test]
    public void NotHaveAVersionForNonVersionedAssembly()
    {
        var version = GitVersion.For(typeof(string).GetTypeInfo().Assembly);
        version.HasVersion.ShouldBeFalse();
    }

    [Test]
    [Skip("Disabled for CI")]
    public void ForTypeInfo()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo());

        version.ShouldNotBeNull();
        version.HasVersion.ShouldBeTrue();
    }

    [Test]
    [Skip("Disabled for CI")]
    public void ForType()
    {
        var version = GitVersion.For(typeof(GitVersion));

        version.ShouldNotBeNull();
        version.HasVersion.ShouldBeTrue();
    }

    [Test]
    public void ForManyTypesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion), typeof(GitVersion));

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Test]
    public void ForManyTypeInfosAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Test]
    public void ForManyAssembliesAndReduce()
    {
        var version = GitVersion.For(typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly);

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Test]
    public void ForManyTypesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion), typeof(GitVersion) }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Test]
    public void ForManyTypeInfosAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo(), typeof(GitVersion).GetTypeInfo() }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }

    [Test]
    public void ForManyAssembliesAndReduceEnumerable()
    {
        var version = GitVersion.For(new[] { typeof(GitVersion).GetTypeInfo().Assembly, typeof(GitVersion).GetTypeInfo().Assembly }.AsEnumerable());

        version.ShouldNotBeNull();
        version.Count.ShouldBe(1);
    }
}
