using Microsoft.Extensions.Configuration;

using Spectre.Console.Testing;

namespace Sample.Command.Tests;

public class CommandLineApplicationTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Run_The_Default_Command_When_No_Command_Is_Given()
    {
        var console = new TestConsole();

        var exitCode = await CommandLineApplication.RunAsync([], console, cancellationToken: TestContext.CancellationToken);

        await exitCode.Should().Because(console.Output).BeEqualTo(0);
        await console.Output.Should().Contain("Hello World!");
    }

    [Test]
    public async Task Should_Dump_The_Configuration_With_A_Command_That_Takes_Its_Services_From_DryIoc()
    {
        var console = new TestConsole();

        var exitCode = await CommandLineApplication.RunAsync(
            ["dump"],
            console,
            builder => builder.Configuration.AddInMemoryCollection([new("Sample:Greeting", "Hello from the test"),]),
            TestContext.CancellationToken
        );

        await exitCode.Should().Because(console.Output).BeEqualTo(0);
        // the configuration the host was started with
        await console.Output.Should().Contain("Sample:Greeting");
        await console.Output.Should().Contain("Hello from the test");
        // InstanceThing is registered in the DryIoc container only, so the command was created by it
        await console.Output.Should().Contain("DryIoc");
    }

    [Test]
    public async Task Should_Not_Run_A_Command_That_Does_Not_Exist()
    {
        var console = new TestConsole();

        var exitCode = await CommandLineApplication.RunAsync(["missing"], console, cancellationToken: TestContext.CancellationToken);

        await exitCode.Should().NotBeEqualTo(0);
    }
}
