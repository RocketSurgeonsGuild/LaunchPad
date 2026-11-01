using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;

using Serilog.Events;
using Serilog.Extensions.Hosting;

namespace Serilog.AspNetCore.Tests.Filters;

public class SerilogLoggingPageFilterTests
{
    [Test]
    public void Should_Add_The_Selected_Page_Handler_To_The_Request_Event()
    {
        var properties = Collect(new() { Name = "Submit", MethodInfo = typeof(SerilogLoggingPageFilterTests).GetMethod(nameof(Collect))! });

        properties.ShouldContain(x => x.Name == "RazorPageHandler" && x.Value.ToString() == "\"Submit\"");
    }

    [Test]
    public void Should_Leave_Out_The_Page_Handler_When_None_Was_Selected()
    {
        Collect(null).ShouldNotContain(x => x.Name == "RazorPageHandler");
    }

    public static IEnumerable<LogEventProperty> Collect(HandlerMethodDescriptor? handler)
    {
        // the context binds the properties through its logger, and the static one (when it has none) binds nothing
        using var logger = new LoggerConfiguration().CreateLogger();
        var diagnosticContext = new DiagnosticContext(logger);
        using var collector = diagnosticContext.BeginCollection();
        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new CompiledPageActionDescriptor());
        var context = new PageHandlerSelectedContext(new PageContext(actionContext), [], new object()) { HandlerMethod = handler };

        new SerilogLoggingPageFilter(diagnosticContext).OnPageHandlerSelected(context);

        collector.TryComplete(out var properties).ShouldBeTrue();
        return [.. properties ?? []];
    }
}
