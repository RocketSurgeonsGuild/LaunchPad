using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Restful.Tests;

internal class ApiDescriptionData<T> : TheoryData<ApiDescriptionData>
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

public class ApiDescriptionData
{
    public ApiDescriptionData(ApiDescription description)
    {
        Description = description;
    }

    public ApiDescription Description { get; }

    public override string ToString()
    {
        return $"[{Description.HttpMethod}] {Description.RelativePath}";
    }
}
