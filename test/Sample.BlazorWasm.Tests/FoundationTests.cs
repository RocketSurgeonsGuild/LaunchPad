using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.BlazorWasm.Tests;

public class FoundationTests : HandleTestHostBase
{
    [Fact]
    public void AutoMapper()
    {
        ServiceProvider.GetRequiredService<IMapper>()
                       .ConfigurationProvider.AssertConfigurationIsValid();
    }

    public FoundationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}
