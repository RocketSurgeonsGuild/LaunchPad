using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Checks that <see cref="LaunchPadHealthCheckOptions.Port" /> is a port number. The <c>[Range]</c> on it describes the configuration
///     schema; this enforces it without reading the attribute through reflection.
/// </summary>
internal sealed class HealthCheckOptionsValidator : IValidateOptions<LaunchPadHealthCheckOptions>
{
    public ValidateOptionsResult Validate(string? name, LaunchPadHealthCheckOptions options) =>
        options.Port is null or ( >= 1 and <= 65535 )
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(
                $"{LaunchPadHealthCheckOptions.SectionName}:{nameof(LaunchPadHealthCheckOptions.Port)} must be a port number from 1 to 65535, not {options.Port}."
            );
}
