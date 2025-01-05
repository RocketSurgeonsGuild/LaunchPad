using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Helper methods for creating <see cref="ExistingValueOptionsFactory{TOptions}" /> instances.
/// </summary>
public static class ExistingValueOptions
{
    /// <summary>
    ///     Applys all of the <see cref="IConfigureOptions{TOptions}" />, <see cref="IPostConfigureOptions{TOptions}" />, and <see cref="IValidateOptions{TOptions}" />
    ///     instances to the options instance.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    /// <param name="name"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <exception cref="OptionsValidationException"></exception>
    public static void Apply<TOptions>(IServiceProvider serviceProvider, TOptions options, string name) where TOptions : class, new() => new ExistingValueOptionsFactory<TOptions>(
        options,
        serviceProvider.GetServices<IConfigureOptions<TOptions>>(),
        serviceProvider.GetServices<IPostConfigureOptions<TOptions>>(),
        serviceProvider.GetServices<IValidateOptions<TOptions>>()
        ).Create(name);
}

/// <summary>
///     Implementation of <see cref="IOptionsFactory{TOptions}" />.
/// </summary>
/// <typeparam name="TOptions">The type of options being requested.</typeparam>
/// <remarks>
///     Initializes a new instance with the specified options configurations.
/// </remarks>
/// <param name="instance">The instance to configure</param>
/// <param name="setups">The configuration actions to run.</param>
/// <param name="postConfigures">The initialization actions to run.</param>
/// <param name="validations">The validations to run.</param>
public class ExistingValueOptionsFactory<TOptions>(
    TOptions instance,
    IEnumerable<IConfigureOptions<TOptions>> setups,
    IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
    IEnumerable<IValidateOptions<TOptions>> validations
    ) : OptionsFactory<TOptions>(setups, postConfigures, validations)
    where TOptions : class, new()
{
    private readonly TOptions _instance = instance;

    /// <inheritdoc />
    protected override TOptions CreateInstance(string name) => _instance;
}
