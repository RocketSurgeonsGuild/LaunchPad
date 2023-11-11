using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core.Tests;

public class FoundationTests(ITestOutputHelper outputHelper) : HandleTestHostBase(outputHelper)
{
    [Fact]
    public void AutoMapper()
    {
        ServiceProvider.GetRequiredService<IMapper>()
                       .ConfigurationProvider.AssertConfigurationIsValid();
    }
}
