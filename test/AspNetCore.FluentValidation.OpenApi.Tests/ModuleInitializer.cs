using System.Runtime.CompilerServices;
using DiffEngine;

namespace AspNetCore.FluentValidation.OpenApi.Tests;

static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DiffRunner.Disabled = true;
        DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode, DiffTool.VisualStudio);
//        Verify
    }
}