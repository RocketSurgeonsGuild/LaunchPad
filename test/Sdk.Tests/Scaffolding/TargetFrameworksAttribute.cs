namespace Sdk.Tests.Scaffolding;

/// <summary>
///     Runs every test in the class once per target framework the repository declares (<see cref="Repository.TargetFrameworks" />), as the
///     target framework of the throwaway projects. The test project itself runs on the newest framework only: MSBuild.ProjectCreation loads
///     MSBuild from an installed SDK of the test host's major version, which is the one global.json pins. The runs share one snapshot.
/// </summary>
/// <remarks>Modelled on the attribute of the same name in Dovetail's SDK tests.</remarks>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class TargetFrameworksAttribute : DataSourceGeneratorAttribute<string>
{
    protected override IEnumerable<Func<string>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        foreach (var targetFramework in Repository.TargetFrameworks)
        {
            yield return () => targetFramework;
        }
    }
}
