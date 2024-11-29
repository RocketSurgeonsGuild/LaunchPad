using FluentValidation;

internal class CustomHostedServiceOptions
{
    public string? A { get; set; }

    [UsedImplicitly]
    private sealed class Validator : AbstractValidator<CustomHostedServiceOptions>
    {
        public Validator()
        {
            RuleFor(z => z.A).NotNull();
        }
    }
}