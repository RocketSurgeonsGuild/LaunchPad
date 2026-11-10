using ZeroAlloc.Mediator.Validation;
using ZeroAlloc.Validation;

namespace ZeroAlloc.AspNetCore.Tests;

internal static class Failures
{
    public static ValidationFailedException Exception(params (string Property, string Message)[] failures) =>
        new(new ValidationError([.. failures.Select(f => new ValidationFailure { PropertyName = f.Property, ErrorMessage = f.Message, ErrorCode = "Code" })]));
}
