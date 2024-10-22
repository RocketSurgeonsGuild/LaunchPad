using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

namespace Rocket.Surgery.LaunchPad.Testing;

/// <summary>
///     A fake clock convention used during unit testing
/// </summary>
[PublicAPI]
[UnitTestConvention]
[ExportConvention]
[BeforeConvention(typeof(NodaTimeConvention))]
[ConventionCategory(ConventionCategory.Core)]
public class FakeClockConvention : IServiceConvention
{
    private readonly int _unixTimeSeconds;
    private readonly Duration _advanceBy;

    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="unixTimeSeconds"></param>
    /// <param name="advanceBy"></param>
    public FakeClockConvention(int? unixTimeSeconds = null, Duration? advanceBy = null)
    {
        _unixTimeSeconds = unixTimeSeconds ?? 1577836800;
        _advanceBy = advanceBy ?? Duration.FromSeconds(1);
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(new FakeTimeProvider(DateTimeOffset.FromUnixTimeSeconds(_unixTimeSeconds)) { AutoAdvanceAmount = _advanceBy.ToTimeSpan() });
        services.AddSingleton(s => s.GetRequiredService<TimeProvider>().ToClock());
    }
}
