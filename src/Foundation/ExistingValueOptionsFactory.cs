using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation;

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

        if (_validations != null)
        {
            var failures = new List<string>();
            foreach (var validate in _validations)
            {
                var result = validate.Validate(name, options);
                if (result.Failed) failures.AddRange(result.Failures);
            }

            if (failures.Count > 0) throw new OptionsValidationException(name, typeof(TOptions), failures);
        }

        return options;
    }
}