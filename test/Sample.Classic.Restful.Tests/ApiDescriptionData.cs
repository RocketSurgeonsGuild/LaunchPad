using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

using TUnit.Core.Interfaces;

namespace Sample.Classic.Restful.Tests;

internal static class ApiDescriptionDataSource<T>
    where T : class, ILaunchPadWebAppFixture, IAsyncInitializer, new()
{
    /// <summary>
    ///     The api descriptions of the web app, read once from a short lived web app for all the tests that use them.
    /// </summary>
    public static IEnumerable<Func<ApiDescriptionData>> Rows() => Descriptions.Value.Select(description => (Func<ApiDescriptionData>)( () => description ));

    private static ApiDescriptionData[] Load()
    {
        using var host = new T();
        host.InitializeAsync().GetAwaiter().GetResult();
        var provider = host.AlbaHost.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
        return [.. provider.ApiDescriptionGroups.Items.SelectMany(z => z.Items).Select(item => new ApiDescriptionData(item)),];
    }

    private static readonly Lazy<ApiDescriptionData[]> Descriptions = new(Load);
}

public sealed class ApiDescriptionData(ApiDescription description)
{
    public ApiDescription Description { get; } = description;

    public override string ToString() => $"[{Description.HttpMethod}] {Description.RelativePath}";
}
