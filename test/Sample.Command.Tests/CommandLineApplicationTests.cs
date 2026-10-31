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

        exitCode.ShouldBe(0, console.Output);
        console.Output.ShouldContain("Hello World!");
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

        exitCode.ShouldBe(0, console.Output);
        // the configuration the host was started with
        console.Output.ShouldContain("Sample:Greeting");
        console.Output.ShouldContain("Hello from the test");
        // InstanceThing is registered in the DryIoc container only, so the command was created by it
        console.Output.ShouldContain("DryIoc");
    }

    [Test]
    public async Task Should_Not_Run_A_Command_That_Does_Not_Exist()
    {
        var console = new TestConsole();

        var exitCode = await CommandLineApplication.RunAsync(["missing"], console, cancellationToken: TestContext.CancellationToken);

        exitCode.ShouldNotBe(0);
    }
}
