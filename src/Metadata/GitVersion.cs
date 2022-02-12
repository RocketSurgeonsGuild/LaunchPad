using System.Reflection;

namespace Rocket.Surgery.LaunchPad.Metadata;

/// <summary>
///     GitVersion.
///     Implements the <see cref="IEquatable{T}" />
/// </summary>
/// <seealso cref="IEquatable{GitVersion}" />
public sealed class GitVersion : IEquatable<GitVersion?>
{
    /// <summary>
    ///     Fors the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>GitVersion.</returns>
    public static GitVersion For(Assembly assembly)
    {
        return new GitVersion(assembly);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(IEnumerable<Assembly> assemblies)
    {
        return assemblies.Distinct().ToDictionary(x => x, For);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(params Assembly[] assemblies)
    {
        return assemblies.Distinct().ToDictionary(x => x, For);
    }

    /// <summary>
    ///     Fors the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>GitVersion.</returns>
    public static GitVersion For(Type type)
    {
        return new GitVersion(type.GetTypeInfo().Assembly);
    }

    /// <summary>
    ///     Fors the specified types.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(IEnumerable<Type> types)
    {
        return For(types.Select(x => x.GetTypeInfo().Assembly).Distinct());
    }

    /// <summary>
    ///     Fors the specified types.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(params Type[] types)
    {
        return For(types.Select(x => x.GetTypeInfo().Assembly).Distinct());
    }

    /// <summary>
    ///     Fors the specified type information.
    /// </summary>
    /// <param name="typeInfo">The type information.</param>
    /// <returns>GitVersion.</returns>
    public static GitVersion For(TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return new GitVersion(typeInfo.Assembly);
    }

    /// <summary>
    ///     Fors the specified type infos.
    /// </summary>
    /// <param name="typeInfos">The type infos.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(IEnumerable<TypeInfo> typeInfos)
    {
        return For(typeInfos.Select(x => x.Assembly).Distinct());
    }

    /// <summary>
    ///     Fors the specified type infos.
    /// </summary>
    /// <param name="typeInfos">The type infos.</param>
    /// <returns>IDictionary{Assembly, GitVersion}.</returns>
    public static IDictionary<Assembly, GitVersion> For(params TypeInfo[] typeInfos)
    {
        return For(typeInfos.Select(x => x.Assembly).Distinct());
    }

    /// <summary>
    ///     Implements the == operator.
    /// </summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(GitVersion version1, GitVersion version2)
    {
        return EqualityComparer<GitVersion>.Default.Equals(version1, version2);
    }

    /// <summary>
    ///     Implements the != operator.
    /// </summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(GitVersion version1, GitVersion version2)
    {
        return !( version1 == version2 );
    }

    private readonly AssemblyMetadataProvider _assemblyMetadata;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GitVersion" /> class.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    private GitVersion(Assembly assembly)
    {
        _assemblyMetadata = new AssemblyMetadataProvider(assembly);
        _assemblyMetadata.Infer(this);
    }

    /// <summary>
    ///     Does this assembly has any of these attributes?
    /// </summary>
    /// <value><c>true</c> if this instance has version; otherwise, <c>false</c>.</value>
    public bool HasVersion => _assemblyMetadata.HasPrefix("GitVersion_");

    /// <summary>
    ///     Gets the major.
    /// </summary>
    /// <value>The major.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public int Major { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the minor.
    /// </summary>
    /// <value>The minor.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public int Minor { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the patch.
    /// </summary>
    /// <value>The patch.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public int Patch { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the pre release tag.
    /// </summary>
    /// <value>The pre release tag.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? PreReleaseTag { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the pre release tag with dash.
    /// </summary>
    /// <value>The pre release tag with dash.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? PreReleaseTagWithDash { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the build meta data.
    /// </summary>
    /// <value>The build meta data.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? BuildMetaData { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the build meta data padded.
    /// </summary>
    /// <value>The build meta data padded.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? BuildMetaDataPadded { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the full build meta data.
    /// </summary>
    /// <value>The full build meta data.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? FullBuildMetaData { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the major minor patch.
    /// </summary>
    /// <value>The major minor patch.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? MajorMinorPatch { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the sem ver.
    /// </summary>
    /// <value>The sem ver.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? SemVer { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the legacy sem ver.
    /// </summary>
    /// <value>The legacy sem ver.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? LegacySemVer { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the legacy sem ver padded.
    /// </summary>
    /// <value>The legacy sem ver padded.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? LegacySemVerPadded { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the assembly sem ver.
    /// </summary>
    /// <value>The assembly sem ver.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? AssemblySemVer { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the full sem ver.
    /// </summary>
    /// <value>The full sem ver.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? FullSemVer { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the informational version.
    /// </summary>
    /// <value>The informational version.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? InformationalVersion { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the name of the branch.
    /// </summary>
    /// <value>The name of the branch.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? BranchName { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the sha.
    /// </summary>
    /// <value>The sha.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? Sha { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the nu get version v2.
    /// </summary>
    /// <value>The nu get version v2.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? NuGetVersionV2 { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the nu get version.
    /// </summary>
    /// <value>The nu get version.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? NuGetVersion { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the commits since version source.
    /// </summary>
    /// <value>The commits since version source.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public int CommitsSinceVersionSource { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the commits since version source padded.
    /// </summary>
    /// <value>The commits since version source padded.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? CommitsSinceVersionSourcePadded { get; [UsedImplicitly] private set; }

    /// <summary>
    ///     Gets the commit date.
    /// </summary>
    /// <value>The commit date.</value>
    [Prefix("GitVersion_")]
    [UsedImplicitly]
    public string? CommitDate { get; [UsedImplicitly] private set; }

#pragma warning disable CA1056 // Uri properties should not be strings
    /// <summary>
    ///     Gets the repository url.
    /// </summary>
    /// <value>The repository URL.</value>
    [UsedImplicitly]
    public string? RepositoryUrl { get; [UsedImplicitly] private set; }
#pragma warning restore CA1056 // Uri properties should not be strings

    /// <summary>
    ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as GitVersion);
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        var hashCode = -1073977946;
        // ReSharper disable NonReadonlyMemberInGetHashCode
        hashCode = ( hashCode * -1521134295 ) + Major.GetHashCode();
        hashCode = ( hashCode * -1521134295 ) + Minor.GetHashCode();
        hashCode = ( hashCode * -1521134295 ) + Patch.GetHashCode();
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(PreReleaseTag ?? string.Empty);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(PreReleaseTagWithDash ?? string.Empty);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(InformationalVersion ?? string.Empty);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(BranchName ?? string.Empty);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(Sha ?? string.Empty);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<string>.Default.GetHashCode(CommitDate ?? string.Empty);
        // ReSharper enable NonReadonlyMemberInGetHashCode
        return hashCode;
    }

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
    public bool Equals(GitVersion? other)
    {
        return other! != null!
            && Major == other.Major
            && Minor == other.Minor
            && Patch == other.Patch
            && PreReleaseTag == other.PreReleaseTag
            && PreReleaseTagWithDash == other.PreReleaseTagWithDash
            && InformationalVersion == other.InformationalVersion
            && BranchName == other.BranchName
            && Sha == other.Sha
            && CommitDate == other.CommitDate;
    }
}
