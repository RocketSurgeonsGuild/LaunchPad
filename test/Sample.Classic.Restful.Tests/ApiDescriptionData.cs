using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Classic.Restful.Tests;

internal sealed class ApiDescriptionData<T> : TheoryData<ApiDescriptionData>
    where T : class, ILaunchPadWebAppFixture, IAsyncLifetime, new()
{
    public ApiDescriptionData()
    {
        var host = new T();
        host.InitializeAsync().GetAwaiter().GetResult();
        var provider = host.AlbaHost.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
        foreach (var item in provider.ApiDescriptionGroups.Items.SelectMany(z => z.Items))
        {
            Add(new ApiDescriptionData(item));
        }
    }
}

public sealed class ApiDescriptionData(ApiDescription description)
{
    public ApiDescription Description { get; } = description;

    public override string ToString()
    {
        return $"[{Description.HttpMethod}] {Description.RelativePath}";
    }
}
