namespace Sdk.Tests.Scaffolding;

/// <summary>
///     Runs every test in the class once per target framework the repository supports, as the target framework of the throwaway projects.
///     The test project itself runs on .NET 11 only: MSBuild.ProjectCreation loads MSBuild from an installed SDK of the test host's major
///     version, and SDK 11 is the only one mise installs. The two runs share one snapshot.
/// </summary>
/// <remarks>Modelled on the attribute of the same name in Dovetail's SDK tests.</remarks>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class TargetFrameworksAttribute : DataSourceGeneratorAttribute<string>
{
    protected override IEnumerable<Func<string>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        yield return () => "net10.0";
        yield return () => "net11.0";
    }
}
