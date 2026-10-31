using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Tests;

public abstract class JointFakeTest() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    protected async Task Init(Action<DovetailContextBuilder>? action = null)
    {
        var builder = TestJoints.CreateBuilder();
        action?.Invoke(builder);

        var context = await DovetailContext.FromAsync(builder);
        Populate(await TestJoints.ApplyServicesAsync(context));
    }
}

/// <summary>
///     Helpers that turn the joints imported into this test assembly into a context and a service collection.
/// </summary>
internal static class TestJoints
{
    /// <summary>
    ///     The imported joints. The generated <c>Imports</c> seed the Specular provider of this test assembly into the context.
    /// </summary>
    public static DovetailContextBuilder CreateBuilder() => Imports.Joints();

    /// <summary>
    ///     Applies the configuration joints, then the service joints, the way a host does.
    /// </summary>
    public static async Task<IServiceCollection> ApplyServicesAsync(IDovetailContext context)
    {
        var configuration = new ConfigurationManager();
        await configuration.ApplyConfiguration(context);
        context.Set<IConfiguration>(configuration);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        await services.ApplyService(context);
        return services;
    }
}
