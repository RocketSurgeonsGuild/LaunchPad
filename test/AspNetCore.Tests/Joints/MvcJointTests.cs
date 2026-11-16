using System.Reflection;


using Joinery.Dovetail;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

namespace AspNetCore.Tests.Joints;

public class MvcJointTests
{
    [Test]
    public async Task Should_Register_Mvc_And_The_Api_Explorer()
    {
        var services = await JointTestContext.ApplyServicesAsync<MvcJoint>();

        await services.Should().Contain(d => d.ServiceType == typeof(ApplicationPartManager));
        await services.Should().Contain(d => d.ServiceType == typeof(IApiDescriptionGroupCollectionProvider));
    }

    [Test]
    public async Task Should_Not_Add_Exception_Filters_To_Mvc()
    {
        var services = await JointTestContext.ApplyServicesAsync<MvcJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var filters = provider
                     .GetRequiredService<IOptions<MvcOptions>>()
                     .Value
                     .Filters
                     .OfType<TypeFilterAttribute>()
                     .Select(z => z.ImplementationType)
                     .ToArray();

        // an operation reports a failure with a union response, so no filter turns exceptions into problems
        await filters.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Add_Application_Parts_For_The_Assemblies_The_Specular_Provider_Finds()
    {
        var assemblies = new[] { typeof(MvcJoint).Assembly, typeof(MvcJointTests).Assembly };

        var services = await JointTestContext.ApplyServicesAsync<MvcJoint>(JointTestContext.CreateProvider(assemblies));

        var parts = GetApplicationParts(services);
        await parts.Should().Contain(assemblies[0]);
        await parts.Should().Contain(assemblies[1]);
    }

    [Test]
    public async Task Should_Add_The_Application_Assembly_As_An_Application_Part_When_The_Generated_Specular_Provider_Is_Used()
    {
        // the provider an application seeds: the generator resolves the scan in MvcJoint to the assemblies that depend on the
        // AspNetCore assembly, and this test assembly is one of them
        var builder = JointTestContext.CreateBuilder(SpecularProvider.Instance).AppendJoint<MvcJoint>();

        var services = await JointTestContext.ApplyServicesAsync(builder);

        await GetApplicationParts(services).Should().Contain(typeof(MvcJointTests).Assembly);
    }

    [Test]
    public async Task Should_Scan_The_Dependencies_Of_The_Aspnetcore_Assembly()
    {
        var provider = JointTestContext.CreateProvider();

        await JointTestContext.ApplyServicesAsync<MvcJoint>(provider);

        Mock.Get(provider)
            .GetAssemblies(Any(), Any(), Any(), Contains("DependenciesFromAssemblyOf<OpenApiJoint>"))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task Should_Exclude_The_Assemblies_The_Options_In_The_Context_Filter_Out()
    {
        var included = typeof(MvcJoint).Assembly;
        var excluded = typeof(MvcJointTests).Assembly;
        var builder = JointTestContext
                     .CreateBuilder(JointTestContext.CreateProvider(included, excluded))
                     .Set(new AspNetCoreMvcOptions { AssemblyPartFilter = assembly => assembly != excluded })
                     .AppendJoint<MvcJoint>();

        var services = await JointTestContext.ApplyServicesAsync(builder);

        var parts = GetApplicationParts(services);
        await parts.Should().Contain(included);
        await parts.Should().NotContain(excluded);
    }

    [Test]
    public async Task Should_Fail_When_The_Context_Has_No_Specular_Provider()
    {
        var builder = JointTestContext.CreateBuilder(null).AppendJoint<MvcJoint>();

        Exception? thrown = null;
        try
        {
            await JointTestContext.ApplyServicesAsync(builder);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        await thrown.Should().NotBeNull();
        await thrown.ToString().Should().Contain(nameof(ISpecularProvider));
    }

    private static Assembly[] GetApplicationParts(IServiceCollection services) => [.. ( (ApplicationPartManager)services
                                                                               .Last(d => d.ServiceType == typeof(ApplicationPartManager))
                                                                               .ImplementationInstance! )
                                                                               .ApplicationParts
                                                                               .OfType<AssemblyPart>()
                                                                               .Select(z => z.Assembly)];
}
