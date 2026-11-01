namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

internal static class Extensions
{

<<<<<<< TODO: Unmerged change from project 'Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore(net11.0)', Before:
    internal static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;
=======
    internal static bool IsNumeric(this object value) => value is int or long or float or double or decimal;
>>>>>>> After
    internal static bool IsNumeric(this object value) => value is int or long or float or double or decimal;
}
