using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

internal class SerilogLoggingPageFilter(IDiagnosticContext diagnosticContext) : IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
        var name = context.HandlerMethod?.Name ?? context.HandlerMethod?.MethodInfo.Name;
        if (name != null) diagnosticContext.Set("RazorPageHandler", name);
    }

    // Required by the interface
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
    }
}
