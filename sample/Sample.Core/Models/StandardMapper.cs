using Riok.Mapperly.Abstractions;

namespace Sample.Core;

[Mapper]
internal static partial class StandardMapper
{
    public static long LongToDouble(double value)
    {
        return Convert.ToInt64(value);
    }

    public static double DoubleToLong(long value)
    {
        return Convert.ToDouble(value);
    }
}