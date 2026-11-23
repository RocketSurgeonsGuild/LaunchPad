using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

/// <summary>
///     Checks a domain primitive through the interface the generator implements, so one helper covers every kind, struct or class.
/// </summary>
internal static class PrimitiveAssert
{
    /// <summary>
    ///     The value is accepted by TryCreate and Create alike, and the domain primitive holds it.
    /// </summary>
    public static Task<TPrimitive> Accepts<TPrimitive, TValue>(TValue value)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull => Accepts<TPrimitive, TValue>(value, value);

    /// <summary>
    ///     The value is accepted by TryCreate and Create alike, and the domain primitive holds <paramref name="expected" />: the value after its
    ///     normalization.
    /// </summary>
    public static async Task<TPrimitive> Accepts<TPrimitive, TValue>(TValue value, TValue expected)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var accepted = TPrimitive.TryCreate(value, out var result, out var error);
        await accepted.Should().Because($"{value} should be accepted, but: [{error.Code}] {error.Message}").BeTrue();
        await error.Should().BeEqualTo(PrimitiveError.None);
        await result!.Value.Should().BeEqualTo(expected);
        await TPrimitive.Create(value).Should().BeEqualTo(result);
        return result;
    }

    /// <summary>
    ///     The value is rejected with <paramref name="code" /> by TryCreate, and Create throws a DomainPrimitiveValidationException with the
    ///     same error.
    /// </summary>
    public static async Task<PrimitiveError> Rejects<TPrimitive, TValue>(TValue value, string code)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var accepted = TPrimitive.TryCreate(value, out var result, out var error);
        await accepted.Should().Because($"{value} should be rejected").BeFalse();
        await error.Code.Should().BeEqualTo(code);
        await error.IsError.Should().BeTrue();
        await Assert.That(result is null || result.IsDefault).IsTrue();

        var exception = await ( (Action)( () => TPrimitive.Create(value) ) ).Should().Throw<DomainPrimitiveValidationException>();
        await exception!.Error.Should().BeEqualTo(error);
        return error;
    }

    public static async Task<TException?> Throws<TException>(Action action)
        where TException : Exception => await action.Should().Throw<TException>();
}
