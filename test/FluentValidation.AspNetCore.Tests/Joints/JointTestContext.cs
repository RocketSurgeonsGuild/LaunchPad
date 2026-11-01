using System.Reflection;

using FakeItEasy;

using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Specular.Abstractions;

namespace FluentValidation.AspNetCore.Tests.Joints;

/// <summary>
///     Creates joint contexts for the joint tests, with a Specular provider seeded the way an application seeds it.
/// </summary>
internal static class JointTestContext
{
    /// <summary>
    ///     A joint context builder with the given Specular provider.
    /// </summary>
    /// <param name="provider">The provider to seed. When it is <see langword="null" /> the builder has no provider at all.</param>
    public static DovetailContextBuilder CreateBuilder(ISpecularProvider? provider)
    {
        // An empty category set loads the Application joints and the Core joints, which is every category the joints under test are in.
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), []);
        return provider is null ? builder : builder.Set(provider);
    }

    /// <summary>
    ///     A joint context builder with a Specular provider that finds no assemblies.
    /// </summary>
    public static DovetailContextBuilder CreateBuilder() => CreateBuilder(CreateProvider());

    /// <summary>
    ///     A fake Specular provider that finds the given assemblies for every assembly scan.
    /// </summary>
    public static ISpecularProvider CreateProvider(params Assembly[] assemblies)
    {
        var provider = A.Fake<ISpecularProvider>();
        A
           .CallTo(() => provider.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
           .Returns(assemblies);
        return provider;
    }

    /// <summary>
    ///     Creates the context from the builder and applies every service joint in it to a new service collection.
    /// </summary>
    public static async Task<ServiceCollection> ApplyServicesAsync(DovetailContextBuilder builder)
    {
        var context = await builder.CreateAsync();
        var services = new ServiceCollection();
        await services.ApplyService(context);
        return services;
    }

    /// <summary>
    ///     Applies a single service joint to a new service collection, in a context that has a Specular provider.
    /// </summary>
    public static Task<ServiceCollection> ApplyServicesAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TJoint>(
        ISpecularProvider? provider = null
    )
        where TJoint : class, IServiceJoint =>
        ApplyServicesAsync(CreateBuilder(provider ?? CreateProvider()).AppendJoint<TJoint>());

    /// <summary>
    ///     Builds a service provider the options and the logging that the registered services expect can be resolved from.
    /// </summary>
    public static ServiceProvider BuildServiceProvider(IServiceCollection services) => services.AddLogging().BuildServiceProvider();
}
