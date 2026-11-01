using System.Reflection;

using FakeItEasy;

using Joinery.Dovetail;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class AspNetCoreJointTests
{
    [Test]
    public async Task Should_Register_Mvc_And_The_Api_Explorer()
    {
        var services = await JointTestContext.ApplyServicesAsync<AspNetCoreJoint>();

        services.ShouldContain(d => d.ServiceType == typeof(ApplicationPartManager));
        services.ShouldContain(d => d.ServiceType == typeof(IApiDescriptionGroupCollectionProvider));
    }

    [Test]
    public async Task Should_Add_The_Launchpad_Filters_To_Mvc()
    {
        var services = await JointTestContext.ApplyServicesAsync<AspNetCoreJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var filters = provider
                     .GetRequiredService<IOptions<MvcOptions>>()
                     .Value
                     .Filters
                     .OfType<TypeFilterAttribute>()
                     .Select(z => z.ImplementationType)
                     .ToArray();

        filters.ShouldContain(typeof(NotFoundExceptionFilter));
        filters.ShouldContain(typeof(NotAuthorizedExceptionFilter));
        filters.ShouldContain(typeof(RequestFailedExceptionFilter));
    }

    [Test]
    public async Task Should_Add_Application_Parts_For_The_Assemblies_The_Specular_Provider_Finds()
    {
        var assemblies = new[] { typeof(AspNetCoreJoint).Assembly, typeof(AspNetCoreJointTests).Assembly };

        var services = await JointTestContext.ApplyServicesAsync<AspNetCoreJoint>(JointTestContext.CreateProvider(assemblies));

        var parts = GetApplicationParts(services);
        parts.ShouldContain(assemblies[0]);
        parts.ShouldContain(assemblies[1]);
    }

    [Test]
    public async Task Should_Add_The_Application_Assembly_As_An_Application_Part_When_The_Generated_Specular_Provider_Is_Used()
    {
        // the provider an application seeds: the generator resolves the scan in AspNetCoreJoint to the assemblies that depend on the
        // AspNetCore assembly, and this test assembly is one of them
        var builder = JointTestContext.CreateBuilder(SpecularProvider.Instance).AppendJoint<AspNetCoreJoint>();

        var services = await JointTestContext.ApplyServicesAsync(builder);

        GetApplicationParts(services).ShouldContain(typeof(AspNetCoreJointTests).Assembly);
    }

    [Test]
    public async Task Should_Scan_The_Dependencies_Of_The_Aspnetcore_Assembly()
    {
        var provider = JointTestContext.CreateProvider();

        await JointTestContext.ApplyServicesAsync<AspNetCoreJoint>(provider);

        A
           .CallTo(
                () => provider.GetAssemblies(
                    A<Action<Specular.Abstractions.IReflectionAssemblySelector>>._,
                    A<int>._,
                    A<string>._,
                    A<string>.That.Contains("DependenciesFromAssemblyOf<AspNetCoreJoint>")
                )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task Should_Exclude_The_Assemblies_The_Options_In_The_Context_Filter_Out()
    {
        var included = typeof(AspNetCoreJoint).Assembly;
        var excluded = typeof(AspNetCoreJointTests).Assembly;
        var builder = JointTestContext
                     .CreateBuilder(JointTestContext.CreateProvider(included, excluded))
                     .Set(new AspNetCoreOptions { AssemblyPartFilter = assembly => assembly != excluded })
                     .AppendJoint<AspNetCoreJoint>();

        var services = await JointTestContext.ApplyServicesAsync(builder);

        var parts = GetApplicationParts(services);
        parts.ShouldContain(included);
        parts.ShouldNotContain(excluded);
    }

    [Test]
    public async Task Should_Fail_When_The_Context_Has_No_Specular_Provider()
    {
        var builder = JointTestContext.CreateBuilder(null).AppendJoint<AspNetCoreJoint>();

        Exception? thrown = null;
        try
        {
            await JointTestContext.ApplyServicesAsync(builder);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        thrown.ShouldNotBeNull();
        thrown.ToString().ShouldContain(nameof(ISpecularProvider));
    }

    private static Assembly[] GetApplicationParts(IServiceCollection services) => [.. services
                                                                               .Last(d => d.ServiceType == typeof(ApplicationPartManager))
                                                                               .ImplementationInstance
                                                                               .ShouldBeOfType<ApplicationPartManager>()
                                                                               .ApplicationParts
                                                                               .OfType<AssemblyPart>()
                                                                               .Select(z => z.Assembly)];
}
