using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Classic.Restful.Tests;

internal class ApiDescriptionData<T> : TheoryData<ApiDescriptionData>
    where T : WebApplicationFactory<Startup>, new()
{
    public ApiDescriptionData()
    {
        using var host = new T();
        var provider = host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
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
