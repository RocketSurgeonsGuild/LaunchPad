using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class ValidationHealthCheckResults
{
    private readonly Dictionary<string, HealthReportEntry> _results = new();

    public ValidationHealthCheckResults(IHostApplicationLifetime hostApplicationLifetime)
    {
        hostApplicationLifetime.ApplicationStarted.Register(() => ApplicationHasStarted = true);
    }

    public IEnumerable<KeyValuePair<string, HealthReportEntry>> Results => _results;
    public bool ApplicationHasStarted { get; internal set; }

    public void AddResult(string optionsTypeName, string optionsName, ValidationResult result)
    {
        var key = optionsTypeName;
        if (optionsName != Options.DefaultName)
        {
            key += $"_{optionsName}";
        }

        if (result.IsValid)
        {
            _results[key] =
                new HealthReportEntry(
                    HealthStatus.Healthy,
                    $"Options Validation {key}",
                    TimeSpan.Zero,
                    null,
                    result
                       .Errors
                       .GroupBy(z => z.PropertyName)
                       .ToDictionary(
                            z => z.Key,
                            z => (object)z.Select(x => x.ToString()).ToArray()
                        ),
                    new[] { "options-validation", key }
                );
        }
        else
        {
            _results[key] =
                new HealthReportEntry(
                    HealthStatus.Unhealthy,
                    $"Options Validation {key}",
                    TimeSpan.Zero,
                    new ValidationException(result.Errors),
                    result
                       .Errors
                       .GroupBy(z => z.PropertyName)
                       .ToDictionary(
                            z => z.Key,
                            z => (object)z.Select(x => x.ToString()).ToArray()
                        ),
                    new[] { "options-validation", "Options Validation", key, optionsTypeName }
                );
        }
    }
}
