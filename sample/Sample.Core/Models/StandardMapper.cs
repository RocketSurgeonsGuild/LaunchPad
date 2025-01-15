using Riok.Mapperly.Abstractions;

namespace Sample.Core.Models;

[Mapper]
internal static partial class StandardMapper
{
    public static long LongToDouble(double value) => Convert.ToInt64(value);

    public static double DoubleToLong(long value) => Convert.ToDouble(value);
}
