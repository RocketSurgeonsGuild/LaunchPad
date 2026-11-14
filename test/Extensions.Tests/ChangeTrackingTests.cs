using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests;

public partial class ChangeTrackingTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task ShouldTrackChanges_For_Classes()
    {
        var tracking = new ClassWithTracking(1);
        await tracking.Name.HasValue.Should().BeFalse();
        tracking.Name = "Test";
        await tracking.Name.HasValue.Should().BeTrue();
    }

    [Test]
    public async Task ShouldResetChanges_For_Classes()
    {
        var tracking = new ClassWithTracking(1)
        {
            Name = "Test",
        };
        tracking.ResetChanges();
        await tracking.Name.HasValue.Should().BeFalse();
    }

    [Test]
    public async Task ShouldApplyChanges_For_Classes()
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

        await instance.Name.Should().BeEqualTo("Test");
    }

    [Test]
    public async Task ShouldCreate_For_Classes()
    {
        var instance = new Class(1)
        {
            Name = "MyName",
            Description = "MyDescription",
        };
        var tracking = ClassWithTracking.TrackChanges(instance);
        tracking.Description = "My New description";
        await tracking.Name.HasValue.Should().BeFalse();
        await tracking.Description.HasValue.Should().BeTrue();
        tracking.ApplyChanges(instance);

        await instance.Name.Should().BeEqualTo("MyName");
        await instance.Description.Should().BeEqualTo("My New description");
    }

    [Test]
    public async Task ShouldTrackChanges_For_Records()
    {
        var tracking = new RecordWithTracking(1);
        await tracking.Name.HasValue.Should().BeFalse();
        tracking.Name = "Test";
        await tracking.Name.HasValue.Should().BeTrue();
    }

    [Test]
    public async Task ShouldResetChanges_For_Records()
    {
        var tracking = new RecordWithTracking(1)
        {
            Name = "Test",
        };
        tracking.ResetChanges();
        await tracking.Name.HasValue.Should().BeFalse();
    }

    [Test]
    public async Task ShouldApplyChanges_For_Records()
    {
        var instance = new Record(1, "MyName");
        var tracking = new RecordWithTracking(1)
        {
            Name = "Test",
        };
        instance = tracking.ApplyChanges(instance);

        await instance.Name.Should().BeEqualTo("Test");
    }

    [Test]
    public async Task ShouldCreate_For_Records()
    {
        var instance = new Record(1, "MyName")
        {
            Description = "MyDescription",
        };
        var tracking = RecordWithTracking.TrackChanges(instance);
        tracking.Description = "My New description";
        await tracking.Name.HasValue.Should().BeFalse();
        await tracking.Description.HasValue.Should().BeTrue();
        instance = tracking.ApplyChanges(instance);

        await instance.Name.Should().BeEqualTo("MyName");
        await instance.Description.Should().BeEqualTo("My New description");
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
