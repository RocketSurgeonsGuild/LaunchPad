using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

/// <summary>
///     This class enables fluent validators to be used for options validations!
/// </summary>
/// <typeparam name="T"></typeparam>
internal class FluentValidationOptions<T> : IValidateOptions<T>
    where T : class
{
    private readonly ValidationHealthCheckResults? _healthCheckResults;
    private readonly IValidator<T>? _validator;

    public FluentValidationOptions(
        ValidationHealthCheckResults? healthCheckResults = null,
        IValidator<T>? validator = null
    )
    {
        _healthCheckResults = healthCheckResults;
        _validator = validator;
    }

    public ValidateOptionsResult Validate(string name, T options)
    {
        if (_validator == null) return ValidateOptionsResult.Skip;

        var result = _validator.Validate(options);
        _healthCheckResults?.AddResult(typeof(T).Name, name, result);
        if (_healthCheckResults is not null) return ValidateOptionsResult.Skip;
        if (result.IsValid) return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            new[] { $"Failure while validating {typeof(T).Name} {( name == Options.DefaultName ? "" : $"(Name: {name})" )}." }
               .Concat(result.Errors.Select(z => z.ToString()))
        );
    }
}

internal class ValidationHealthCheckResults
{
    private readonly Dictionary<string, HealthReportEntry> _results = new();

    public IEnumerable<KeyValuePair<string, HealthReportEntry>> Results => _results;

    public void AddResult(string optionsTypeName, string optionsName, ValidationResult result)
    {
        var key = optionsTypeName;
        if (optionsName != Options.DefaultName)
        {
            key += $"(Name: {optionsName})";
        }

        if (result.IsValid)
        {
            _results.Add(
                key,
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
                    new[] { "options-validation", "Options Validation", key, optionsTypeName }
                )
            );
        }
        else
        {
            _results.Add(
                key,
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
                )
            );
        }
    }
}

internal class CustomHealthCheckService : HealthCheckService
{
    private readonly HealthCheckService _wrappedService;
    private readonly ValidationHealthCheckResults _healthCheckResults;

    public CustomHealthCheckService(HealthCheckService wrappedService, ValidationHealthCheckResults healthCheckResults)
    {
        _wrappedService = wrappedService;
        _healthCheckResults = healthCheckResults;
    }

    public override async Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        var results = await _wrappedService.CheckHealthAsync(predicate, cancellationToken);
        return new HealthReport(
            results.Entries.Concat(_healthCheckResults.Results).ToDictionary(z => z.Key, z => z.Value),
            results.TotalDuration
        );
    }
}
