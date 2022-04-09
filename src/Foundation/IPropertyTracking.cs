using MediatR;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IPropertyTracking<T>
{
    /// <summary>
    ///     Method used apply changes from a given property tracking class.
    /// </summary>
    /// <remarks>
    ///     For records this will return a new record instance for classes it will mutate the existing instance.
    /// </remarks>
    /// <param name="state"></param>
    /// <returns></returns>
    T ApplyChanges(T state);

    /// <summary>
    ///     Reset the state of the property tracking to only track future changes
    /// </summary>
    /// <returns></returns>
    void ResetChanges();
}

/// <summary>
///     A helper class for tracking changes to a value
/// </summary>
/// <typeparam name="T"></typeparam>
public class Assigned<T>
{
    private T _value;
    private bool _hasBeenSet;

    /// <summary>
    ///     The constructor for creating an assigned value
    /// </summary>
    /// <param name="value"></param>
    public Assigned(T value)
    {
        _value = value;
    }

    /// <summary>
    ///     The underlying value
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            _hasBeenSet = true;
            _value = value;
        }
    }

    /// <summary>
    ///     Has the value been assigned for this item
    /// </summary>
    public bool HasBeenSet()
    {
        return _hasBeenSet;
    }

    /// <summary>
    ///     Resets this value as changed.
    /// </summary>
    public void ResetState()
    {
        _hasBeenSet = false;
    }

#pragma warning disable CA2225
    /// <summary>
    ///     Implicit operator for returning the underlying value
    /// </summary>
    /// <param name="assigned"></param>
    /// <returns></returns>
    public static implicit operator T?(Assigned<T>? assigned)
    {
        return assigned == null ? default : assigned.Value;
    }

    /// <summary>
    ///     Implicit operator for creating an assigned value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Assigned<T>(T value)
    {
        return new(value);
    }
#pragma warning restore CA2225
}

/// <summary>
///     A common handler for creating patch methods
/// </summary>
/// <typeparam name="TRequest">The request type that will be run after the patch has been applied</typeparam>
/// <typeparam name="TPatch">The patch object itself</typeparam>
/// <typeparam name="TResponse">The final result object</typeparam>
public abstract class PatchHandlerBase<TRequest, TPatch, TResponse> : IRequestHandler<TPatch, TResponse>
    where TRequest : IRequest<TResponse>
    where TPatch : IPropertyTracking<TRequest>, IRequest<TResponse>
{
    private readonly IMediator _mediator;

    /// <summary>
    ///     The based handler using Mediator
    /// </summary>
    /// <param name="mediator"></param>
    protected PatchHandlerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Method used to get <see cref="TRequest" />, database calls, etc.
    /// </summary>
    /// <param name="patchRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TRequest> GetRequest(TPatch patchRequest, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<TResponse> Handle(TPatch request, CancellationToken cancellationToken)
    {
        var underlyingRequest = await GetRequest(request, cancellationToken);
        return await _mediator.Send(request.ApplyChanges(underlyingRequest), cancellationToken);
    }
}
