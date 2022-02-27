namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     HotChocolate options
/// </summary>
public class RocketChocolateOptions
{
    /// <summary>
    ///     Include the assembly info query data automagically
    /// </summary>
    public bool IncludeAssemblyInfoQuery { get; set; }

    /// <summary>
    ///     A check that can be used to select specific MediatR requests that will be converted to mutations.
    /// </summary>
    public Func<Type, bool> RequestPredicate { get; set; } = z => z is { IsNested: true, DeclaringType: { } };
}
