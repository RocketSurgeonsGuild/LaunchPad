using System.Runtime.CompilerServices;

using DiffEngine;

using VerifyTests.DiffPlex;

namespace FluentValidation.AspNetCore.Tests.OpenApi;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DiffRunner.Disabled = true;
        DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode, DiffTool.VisualStudio);
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.SortPropertiesAlphabetically();
        //        Verify
    }
}
