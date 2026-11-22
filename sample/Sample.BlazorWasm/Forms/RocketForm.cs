using Rocket.Surgery.LaunchPad.Foundation;

using ZeroAlloc.Validation;

namespace Sample.BlazorWasm.Forms;

/// <summary>
///     A rocket as the create and edit forms send it to Sample.Minimal, validated in the browser with the rules Sample.Core's CreateRocket and
///     EditRocket requests apply on the server.
/// </summary>
[Validate]
public readonly record struct RocketRequest
{
    /// <summary>The serial number of the rocket.</summary>
    [NotEmpty]
    [MinLength(10)]
    [MaxLength(30)]
    public string SerialNumber { get; init; }

    /// <summary>The type of the rocket.</summary>
    [IsInEnum]
    public RocketType Type { get; init; }

    /// <summary>The rocket <paramref name="model" /> describes, to edit.</summary>
    public static RocketRequest From(RocketModel model) => new() { SerialNumber = model.Sn, Type = model.Type };

    /// <summary>The body of the create request.</summary>
    public CreateRocketRequest ToCreateRequest() => new() { SerialNumber = SerialNumber, Type = Type };

    /// <summary>The body of the edit request of the rocket <paramref name="id" />.</summary>
    public EditRocketRequest ToEditRequest(Guid id) => new() { Id = id, SerialNumber = SerialNumber, Type = Type };
}

/// <summary>
///     The <see cref="RocketRequest" /> a Blazor form binds to: a readonly request can't be edited a field at a time.
/// </summary>
[Mutable<RocketRequest>]
public partial class RocketForm;
