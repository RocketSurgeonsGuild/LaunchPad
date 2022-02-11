namespace Rocket.Surgery.LaunchPad.Metadata;

/// <summary>
///     PrefixAttribute.
///     Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property)]
internal sealed class PrefixAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PrefixAttribute" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public PrefixAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    ///     Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key { get; }
}
