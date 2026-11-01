namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

internal static class Extensions
{
    internal static bool IsNumeric(this object value) => value is int or long or float or double or decimal;
}
