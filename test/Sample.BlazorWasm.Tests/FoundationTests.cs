using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Sample.BlazorWasm.Tests
{
    public class FoundationTests : HandleTestHostBase
    {
        public FoundationTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void AutoMapper() => ServiceProvider.GetRequiredService<IMapper>()
           .ConfigurationProvider.AssertConfigurationIsValid();
    }
}