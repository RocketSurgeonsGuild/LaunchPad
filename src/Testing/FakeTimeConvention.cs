using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

using NodaTime;
using NodaTime.Extensions;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Primitives.Conventions;

namespace Rocket.Surgery.LaunchPad.Testing;

/// <summary>
///     A fake clock convention used during unit testing
/// </summary>
/// <remarks>
///     The constructor
/// </remarks>
/// <param name="unixTimeSeconds"></param>
/// <param name="advanceBy"></param>
[PublicAPI]
[UnitTestConvention]
[ExportConvention]
[BeforeConvention<TimeConvention>]
[ConventionCategory(ConventionCategory.Core)]
public class FakeTimeConvention(int? unixTimeSeconds = null, Duration? advanceBy = null) : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.RemoveAll<TimeProvider>();
        services.AddSingleton(new FakeTimeProvider(DateTimeOffset.FromUnixTimeSeconds(_unixTimeSeconds)) { AutoAdvanceAmount = _advanceBy.ToTimeSpan() });
        services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<FakeTimeProvider>());
        services.AddSingleton(s => s.GetRequiredService<TimeProvider>().ToClock());
    }

    private readonly int _unixTimeSeconds = unixTimeSeconds ?? 1577836800;
    private readonly Duration _advanceBy = advanceBy ?? Duration.FromSeconds(1);
}
