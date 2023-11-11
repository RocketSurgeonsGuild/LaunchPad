namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

/// <summary>
///     Validation information for grpc
/// </summary>
[Serializable]
public class ValidationTrailers
{
    // ReSharper disable once NullableWarningSuppressionIsUsed
    /// <summary>
    ///     The property name
    /// </summary>
    public string PropertyName { get; set; } = null!;

    // ReSharper disable once NullableWarningSuppressionIsUsed
    /// <summary>
    ///     The error message
    /// </summary>
    public string ErrorMessage { get; set; } = null!;

    // ReSharper disable once NullableWarningSuppressionIsUsed
    /// <summary>
    ///     The given value
    /// </summary>
    public object AttemptedValue { get; set; } = null!;
}
