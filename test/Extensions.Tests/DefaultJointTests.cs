using Joinery.Dovetail;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Extensions.Tests;

public class DefaultJointTests() : JointFakeTest()
{
    [Test]
    public async Task Should_Register_Services_Marked_With_The_Specular_Service_Registration_Attributes()
    {
        await Init();

        ServiceProvider.GetRequiredService<ISingletonThing>().ShouldBeOfType<SingletonThing>();
        ServiceProvider.GetRequiredService<ITransientThing>().ShouldBeOfType<TransientThing>();
        ServiceProvider.GetRequiredKeyedService<IKeyedThing>("keyed").ShouldBeOfType<KeyedThing>();
    }

    [Test]
    public async Task Should_Register_Options_And_Logging()
    {
        await Init();

        ServiceProvider.GetService<IOptions<object>>().ShouldNotBeNull();
        ServiceProvider.GetService<ILoggerFactory>().ShouldNotBeNull();
    }

    [Test]
    public async Task Should_Say_The_Specular_Provider_Is_Missing_When_It_Was_Not_Set()
    {
        // A bare context: the generated Imports seed the provider since Dovetail 0.3.1, so they cannot be used to leave it out.
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), []);
        var context = await DovetailContext.FromAsync(builder);

        var exception = Should.Throw<KeyNotFoundException>(() => new DefaultJoint().Register(context, new ServiceCollection()));

        exception.Message.ShouldContain(nameof(ISpecularProvider));
    }

    [Test]
    public async Task Should_Find_The_Specular_Provider_Without_Setting_It_Because_The_Generated_Imports_Seed_It()
    {
        var context = await DovetailContext.FromAsync(Imports.Joints());

        context.Get<ISpecularProvider>().ShouldNotBeNull();
    }

    public interface ISingletonThing;

    public interface ITransientThing;

    public interface IKeyedThing;

    [ServiceRegistration(ServiceLifetime.Singleton, typeof(ISingletonThing))]
    public class SingletonThing : ISingletonThing;

    [ServiceRegistration(ServiceLifetime.Transient, typeof(ITransientThing))]
    public class TransientThing : ITransientThing;

    [KeyedServiceRegistration<IKeyedThing>("keyed", ServiceLifetime.Singleton)]
    public class KeyedThing : IKeyedThing;
}
