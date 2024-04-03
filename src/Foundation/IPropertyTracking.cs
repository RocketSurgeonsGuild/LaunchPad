namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
/// </summary>
public interface IPropertyTracking;

/// <summary>
///     Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IPropertyTracking<T> : IPropertyTracking
{
    /// <summary>
    ///     Method used apply changes from a given property tracking class.
    /// </summary>
    /// <remarks>
    ///     For records this will return a new record instance for classes it will mutate the existing instance.
    /// </remarks>
    /// <param name="state"></param>
    /// <returns></returns>
    T ApplyChanges(T state);

    /// <summary>
    ///     Reset the state of the property tracking to only track future changes
    /// </summary>
    /// <returns></returns>
    void ResetChanges();
}