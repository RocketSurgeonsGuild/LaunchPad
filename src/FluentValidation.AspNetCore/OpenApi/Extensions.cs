namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

internal static class Extensions
{
    internal static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;
}