using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests;

public partial class ChangeTrackingTests(ITestOutputHelper testOutputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper))
{
    [Fact]
    public void ShouldTrackChanges_For_Classes()
    {
        var tracking = new ClassWithTracking(1);
        tracking.Name.HasValue.Should().BeFalse();
        tracking.Name = "Test";
        tracking.Name.HasValue.Should().BeTrue();
    }

    [Fact]
    public void ShouldResetChanges_For_Classes()
    {
        var tracking = new ClassWithTracking(1)
        {
            Name = "Test",
        };
        tracking.ResetChanges();
        tracking.Name.HasValue.Should().BeFalse();
    }

    [Fact]
    public void ShouldApplyChanges_For_Classes()
    {
        var instance = new Class(1)
        {
            Name = "MyName",
        };
        var tracking = new ClassWithTracking(1)
        {
            Name = "Test",
        };
        tracking.ApplyChanges(instance);

        instance.Name.Should().Be("Test");
    }

    [Fact]
    public void ShouldCreate_For_Classes()
    {
        var instance = new Class(1)
        {
            Name = "MyName",
            Description = "MyDescription",
        };
        var tracking = ClassWithTracking.TrackChanges(instance);
        tracking.Description = "My New description";
        tracking.Name.HasValue.Should().BeFalse();
        tracking.Description.HasValue.Should().BeTrue();
        tracking.ApplyChanges(instance);

        instance.Name.Should().Be("MyName");
        instance.Description.Should().Be("My New description");
    }

    [Fact]
    public void ShouldTrackChanges_For_Records()
    {
        var tracking = new RecordWithTracking(1);
        tracking.Name.HasValue.Should().BeFalse();
        tracking.Name = "Test";
        tracking.Name.HasValue.Should().BeTrue();
    }

    [Fact]
    public void ShouldResetChanges_For_Records()
    {
        var tracking = new RecordWithTracking(1)
        {
            Name = "Test",
        };
        tracking.ResetChanges();
        tracking.Name.HasValue.Should().BeFalse();
    }

    [Fact]
    public void ShouldApplyChanges_For_Records()
    {
        var instance = new Record(1, "MyName");
        var tracking = new RecordWithTracking(1)
        {
            Name = "Test",
        };
        instance = tracking.ApplyChanges(instance);

        instance.Name.Should().Be("Test");
    }

    [Fact]
    public void ShouldCreate_For_Records()
    {
        var instance = new Record(1, "MyName")
        {
            Description = "MyDescription",
        };
        var tracking = RecordWithTracking.TrackChanges(instance);
        tracking.Description = "My New description";
        tracking.Name.HasValue.Should().BeFalse();
        tracking.Description.HasValue.Should().BeTrue();
        instance = tracking.ApplyChanges(instance);

        instance.Name.Should().Be("MyName");
        instance.Description.Should().Be("My New description");
    }

    public partial class Class(int Id)
    {
        public int Id { get; } = Id;
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public partial class ClassWithTracking(int Id) : IPropertyTracking<Class>
    {
        public int Id { get; } = Id;
    }

    public partial record Record(int Id, string? Name)
    {
        public string? Description { get; set; }
    }

    public partial record RecordWithTracking(int Id) : IPropertyTracking<Record>;
}
