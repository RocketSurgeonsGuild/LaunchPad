using Microsoft.Extensions.Configuration;

namespace Sample.Command.Tests;

/// <summary>
///     Runs the sample in-process through its hand-written entry point. ConsoleAppFramework keeps its service provider and the exit code in
///     statics, so the runs take turns.
/// </summary>
[NotInParallel(nameof(ProgramTests))]
public class ProgramTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Run_The_Default_Command_When_No_Command_Is_Given()
    {
        using var output = new StringWriter();

        var exitCode = await Program.RunAsync([], output, cancellationToken: TestContext.CancellationToken);

        await exitCode.Should().Because(output.ToString()).BeEqualTo(0);
        await output.ToString().Should().Contain("Hello World!");
    }

    [Test]
    public async Task Should_Dump_The_Configuration_With_A_Command_That_Takes_Its_Services_From_DryIoc()
    {
        using var output = new StringWriter();

        var exitCode = await Program.RunAsync(
            ["dump"],
            output,
            builder => builder.Configuration.AddInMemoryCollection([new("Sample:Greeting", "Hello from the test"),]),
            TestContext.CancellationToken
        );

        await exitCode.Should().Because(output.ToString()).BeEqualTo(0);
        // the configuration the host was started with
        await output.ToString().Should().Contain("Sample:Greeting");
        await output.ToString().Should().Contain("Hello from the test");
        // InstanceThing is registered in the DryIoc container only, by an IDryIocJoint, so the command was created by it
        await output.ToString().Should().Contain("From: DryIoc");
    }

    [Test]
    public async Task Should_Not_Run_A_Command_That_Does_Not_Exist()
    {
        using var output = new StringWriter();

        var exitCode = await Program.RunAsync(["missing"], output, cancellationToken: TestContext.CancellationToken);

        await exitCode.Should().NotBeEqualTo(0);
        await output.ToString().Should().NotContain("Hello World!");
    }
}
