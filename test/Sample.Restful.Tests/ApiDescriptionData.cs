using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Sample.Restful.Tests
{
    public class ApiDescriptionData<T>  : TheoryData<string, ApiDescription>
        where T : WebApplicationFactory<Startup>, new()
    {
        public ApiDescriptionData()
        {
            using var host = new T();
            var provider = host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            foreach (var item in provider.ApiDescriptionGroups.Items.SelectMany(z => z.Items))
            {
                Add(item.ActionDescriptor.DisplayName, item);
            }
        }
    }
}