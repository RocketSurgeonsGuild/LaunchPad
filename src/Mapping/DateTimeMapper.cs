using Riok.Mapperly.Abstractions;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///  A mapper used to map between <see cref="DateTimeOffset" /> and <see cref="DateTime" />
/// </summary>
/// <remarks>
/// If you're using this type... you know what you're doing or you're doing it wrong.
/// </remarks>
[Mapper, PublicAPI]
public partial class DateTimeMapper
{
    /// <summary>
    ///   Converts a <see cref="DateTime" /> to a <see cref="DateTimeOffset" />.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DateTime FromDateTimeOffset(DateTimeOffset source) => source.UtcDateTime;
/// <summary>
///  Converts a <see cref="DateTimeOffset" /> to a <see cref="DateTime" />.
/// </summary>
/// <param name="source"></param>
/// <returns></returns>
    public static DateTimeOffset ToDateTimeOffset(DateTime source) =>
        source switch
        {
            { Kind: DateTimeKind.Unspecified or DateTimeKind.Local } => new(source.ToUniversalTime(), TimeSpan.Zero),
            { Kind: DateTimeKind.Utc }                               => new(source, TimeSpan.Zero),
        };
}
