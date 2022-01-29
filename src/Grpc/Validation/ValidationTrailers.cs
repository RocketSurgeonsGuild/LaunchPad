namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

/// <summary>
///     Validation information for grpc
/// </summary>
[Serializable]
public class ValidationTrailers
{
    /// <summary>
    ///     The property name
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    ///     The error message
    /// </summary>
    public string ErrorMessage { get; set; } = null!;

    /// <summary>
    ///     The given value
    /// </summary>
    public object AttemptedValue { get; set; } = null!;
}
