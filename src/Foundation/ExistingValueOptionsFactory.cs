using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
/// Helper methods for creating <see cref="ExistingValueOptionsFactory{TOptions}"/> instances.
/// </summary>
public static class ExistingValueOptions
{
    /// <summary>
    /// Applys all of the <see cref="IConfigureOptions{TOptions}"/>, <see cref="IPostConfigureOptions{TOptions}"/>, and <see cref="IValidateOptions{TOptions}"/> instances to the options instance.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    /// <param name="name"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <exception cref="OptionsValidationException"></exception>
    public static void Apply<TOptions>(IServiceProvider serviceProvider, TOptions options, string name) where TOptions : class, new()
    {
        var setups = serviceProvider.GetServices<IConfigureOptions<TOptions>>();
        var postConfigures = serviceProvider.GetServices<IPostConfigureOptions<TOptions>>();
        var validations = serviceProvider.GetServices<IValidateOptions<TOptions>>();

        foreach (var setup in setups)
        {
            if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                namedSetup.Configure(name, options);
            else if (name == Options.DefaultName) setup.Configure(options);
        }

        foreach (var post in postConfigures)
        {
            post.PostConfigure(name, options);
        }

        var failures = new List<string>();
        foreach (var validate in validations)
        {
            var result = validate.Validate(name, options);
            if (result.Failed) failures.AddRange(result.Failures);
        }

        if (failures.Count > 0) throw new OptionsValidationException(name, typeof(TOptions), failures);
    }
}

/// <summary>
///     Implementation of <see cref="IOptionsFactory{TOptions}" />.
/// </summary>
/// <typeparam name="TOptions">The type of options being requested.</typeparam>
public class ExistingValueOptionsFactory<TOptions> :
    IOptionsFactory<TOptions>
    where TOptions : class, new()
{
    private readonly TOptions _instance;
    private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
    private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
    private readonly IEnumerable<IValidateOptions<TOptions>>? _validations;

    /// <summary>
    ///     Initializes a new instance with the specified options configurations.
    /// </summary>
    /// <param name="instance">The instance to configure</param>
    /// <param name="setups">The configuration actions to run.</param>
    /// <param name="postConfigures">The initialization actions to run.</param>
    /// <param name="validations">The validations to run.</param>
    public ExistingValueOptionsFactory(
        TOptions instance,
        IEnumerable<IConfigureOptions<TOptions>> setups,
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
        IEnumerable<IValidateOptions<TOptions>> validations
    )
    {
        _instance = instance;
        _setups = setups;
        _postConfigures = postConfigures;
        _validations = validations;
    }

    /// <summary>
    ///     Returns a configured <typeparamref name="TOptions" /> instance with the given <paramref name="name" />.
    /// </summary>
    public TOptions Create(string name)
    {
        var options = _instance;
        foreach (var setup in _setups)
        {
            if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                namedSetup.Configure(name, options);
            else if (name == Options.DefaultName) setup.Configure(options);
        }

        foreach (var post in _postConfigures)
        {
            post.PostConfigure(name, options);
        }

        if (_validations is null) return options;
        var failures = new List<string>();
        foreach (var validate in _validations)
        {
            var result = validate.Validate(name, options);
            if (result.Failed) failures.AddRange(result.Failures);
        }

        if (failures.Count > 0) throw new OptionsValidationException(name, typeof(TOptions), failures);

        return options;
    }
}
