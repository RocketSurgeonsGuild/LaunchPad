using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;

using Serilog.Extensions.Hosting;

namespace Serilog.AspNetCore.Tests.Joints;

public class SerilogRequestLoggingJointTests
{
    [Test]
    public void Is_Exported_As_An_Application_Joint_That_Runs_After_The_AspNetCore_Joint()
    {
        var export = Exports.Joints().Single(x => x.Joint is SerilogRequestLoggingJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(AspNetCoreJoint))
        );
        typeof(SerilogRequestLoggingJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Should_Add_The_Request_Logging_Filters_To_Mvc()
    {
        var services = await JointTestContext.ApplyServicesAsync<SerilogRequestLoggingJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        var filters = provider
                     .GetRequiredService<IOptions<MvcOptions>>()
                     .Value
                     .Filters
                     .OfType<TypeFilterAttribute>()
                     .ToArray();

        filters.ShouldContain(x => x.ImplementationType == typeof(SerilogLoggingActionFilter) && x.Order == 0);
        filters.ShouldContain(x => x.ImplementationType == typeof(SerilogLoggingPageFilter) && x.Order == 0);
    }

    [Test]
    public async Task Should_Register_The_Diagnostic_Context_The_Filters_And_The_Middleware_Write_To()
    {
        var services = await JointTestContext.ApplyServicesAsync<SerilogRequestLoggingJoint>();
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IDiagnosticContext>().ShouldBeSameAs(provider.GetRequiredService<DiagnosticContext>());
    }

    [Test]
    public async Task Should_Keep_A_Diagnostic_Context_That_Is_Already_Registered()
    {
        var existing = new DiagnosticContext(null);
        var services = new ServiceCollection().AddSingleton(existing);
        var context = await JointTestContext.CreateBuilder().AppendJoint<SerilogRequestLoggingJoint>().CreateAsync();
        await services.ApplyService(context);
        using var provider = JointTestContext.BuildServiceProvider(services);

        provider.GetRequiredService<IDiagnosticContext>().ShouldBeSameAs(existing);
    }
}
