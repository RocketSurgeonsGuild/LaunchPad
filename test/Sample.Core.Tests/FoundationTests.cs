using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core.Tests;

public class FoundationTests(ITestOutputHelper outputHelper) : HandleTestHostBase(outputHelper)
{
}
