using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Raise;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
public sealed class ExperimentalRaiseAccumulateApiAttribute : Attribute;

/// <summary>Result of an accumulating computation.</summary>
public abstract class AccumulateValue;

[ExperimentalRaiseAccumulateApi]
public sealed class AccumulateOk<A>(A value) : AccumulateValue
{
    public A Value => value;
}

[ExperimentalRaiseAccumulateApi]
public sealed class AccumulateError(Func<Exception> raise) : AccumulateValue
{
    [DoesNotReturn]
    public Never GetValue() => throw raise();
}

public interface IAccumulate<Error>
{
    AccumulateError Accumulate(Error error) => (AccumulateError)AccumulateAll(error.Nel());

    AccumulateValue AccumulateAll(NonEmptyList<Error> errors);

    bool HasAccumulatedErrors => LatestError is not null;

    AccumulateError? LatestError { get; }

    [ExperimentalRaiseAccumulateApi]
    A GetOrAccumulate<A>(Either<Error, A> either, Func<Error, A> recover) =>
        either.Fold(
            e =>
            {
                Accumulate(e);
                return recover(e);
            },
            static a => a);
}

internal sealed class AccumulatingStoppedException : Exception
{
    private AccumulatingStoppedException() { }

    internal static readonly AccumulatingStoppedException Instance = new();
}

internal sealed class InterruptRaise : IRaise<AccumulateValue>
{
    [DoesNotReturn]
    public void Raise(AccumulateValue error) => throw AccumulatingStoppedException.Instance;
}

[ExperimentalRaiseAccumulateApi]
public sealed class RaiseAccumulate<Error> : IAccumulate<Error>, IRaise<Error>
{
    private readonly IAccumulate<Error> _accumulate;
    private readonly IRaise<NonEmptyList<Error>> _nelRaise;

    [ExperimentalRaiseAccumulateApi]
    public RaiseAccumulate(IRaise<NonEmptyList<Error>> raise)
        : this(new ListAccumulate<Error>(raise))
    {
    }

    internal RaiseAccumulate(ListAccumulate<Error> listAccumulate)
        : this(listAccumulate, listAccumulate, listAccumulate.RaiseSingle)
    {
    }

    internal RaiseAccumulate(
        IAccumulate<Error> accumulate,
        IRaise<NonEmptyList<Error>> nelRaise,
        Func<Error, Exception> raiseErrorsWith)
    {
        _accumulate = accumulate;
        _nelRaise = nelRaise;
        RaiseErrorsWith = raiseErrorsWith;
    }

    internal Func<Error, Exception> RaiseErrorsWith { get; }

    public IRaise<NonEmptyList<Error>> NelRaise => _nelRaise;

    AccumulateValue IAccumulate<Error>.AccumulateAll(NonEmptyList<Error> errors) =>
        _accumulate.AccumulateAll(errors);

    AccumulateError? IAccumulate<Error>.LatestError => _accumulate.LatestError;

    [DoesNotReturn]
    public void Raise(Error error) => throw RaiseErrorsWith(error);

    
    public Dictionary<K, A> BindAll<K, A>(IReadOnlyDictionary<K, Either<Error, A>> map) where K : notnull =>
        _nelRaise.MapValuesOrAccumulate(map, static (acc, entry) => acc.Bind(entry.Value));

    
    public List<A> BindAll<A>(IEnumerable<Either<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, e) => acc.Bind(e));

    
    public NonEmptyList<A> BindAll<A>(NonEmptyList<Either<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, e) => acc.Bind(e));

    
    public NonEmptyList<A> BindAll<A>(NonEmptySet<Either<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, e) => acc.Bind(e));

    
    public A BindNel<A>(Either<NonEmptyList<Error>, A> either) =>
        RaiseExtensions.WithError<NonEmptyList<Error>, NonEmptyList<Error>, A>(
            _nelRaise,
            static e => e,
            nel => RaiseExtensions.Bind(nel, either));

    
    public A WithNel<A>(Func<IRaise<NonEmptyList<Error>>, A> block) => block(_nelRaise);

    
    public List<B> MapOrAccumulate<A, B>(IEnumerable<A> iterable, Func<RaiseAccumulate<Error>, A, B> transform) =>
        _nelRaise.MapOrAccumulate(iterable, transform);

    
    public NonEmptyList<B> MapOrAccumulate<A, B>(
        NonEmptyList<A> list,
        Func<RaiseAccumulate<Error>, A, B> transform) =>
        _nelRaise.MapOrAccumulate(list, transform);

    
    public NonEmptyList<B> MapOrAccumulate<A, B>(
        NonEmptySet<A> set,
        Func<RaiseAccumulate<Error>, A, B> transform) =>
        _nelRaise.MapOrAccumulate(set, transform);

    
    public Dictionary<K, B> MapValuesOrAccumulate<K, A, B>(
        IReadOnlyDictionary<K, A> map,
        Func<RaiseAccumulate<Error>, KeyValuePair<K, A>, B> transform) where K : notnull =>
        _nelRaise.MapValuesOrAccumulate(map, transform);

    public bool HasAccumulatedErrors => _accumulate.HasAccumulatedErrors;

    public AccumulateError? LatestError => _accumulate.LatestError;

    [ExperimentalRaiseAccumulateApi]
    public AccumulateValue Accumulating<A>(Func<RaiseAccumulate<Error>, A> block) =>
        _accumulate.Accumulating(block);

    [ExperimentalRaiseAccumulateApi]
    public AccumulateValue EnsureOrAccumulate(bool condition, Func<Error> raise) =>
        _accumulate.EnsureOrAccumulate(condition, raise);

    [ExperimentalRaiseAccumulateApi]
    public AccumulateValue EnsureNotNullOrAccumulate<B>(B? value, Func<Error> raise) where B : class =>
        _accumulate.EnsureNotNullOrAccumulate(value, raise);

    [ExperimentalRaiseAccumulateApi]
    
    public A Recover<A>(Func<RaiseAccumulate<Error>, A> block, Func<Error, A> recover) =>
        RaiseExtensions.Recover(
            raise => WithNel(nel => block(new RaiseAccumulate<Error>(_accumulate, nel, RaiseErrorsWith))),
            recover);

    [ExperimentalRaiseAccumulateApi]
    
    public List<A> BindAllIor<A>(IEnumerable<Ior<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, ior) => acc.BindIor(ior));

    [ExperimentalRaiseAccumulateApi]
    
    public NonEmptyList<A> BindAllIor<A>(NonEmptyList<Ior<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, ior) => acc.BindIor(ior));

    [ExperimentalRaiseAccumulateApi]
    
    public NonEmptyList<A> BindAllIor<A>(NonEmptySet<Ior<Error, A>> values) =>
        MapOrAccumulate(values, static (acc, ior) => acc.BindIor(ior));

    [ExperimentalRaiseAccumulateApi]
    
    public Dictionary<K, V> BindAllIor<K, V>(IReadOnlyDictionary<K, Ior<Error, V>> map) where K : notnull =>
        MapValuesOrAccumulate(map, static (acc, entry) => acc.BindIor(entry.Value));

    [ExperimentalRaiseAccumulateApi]
    
    public A BindIor<A>(Ior<Error, A> ior) =>
        ior switch
        {
            Ior<Error, A>.Left left => throw RaiseErrorsWith(left.Value),
            Ior<Error, A>.Right right => right.Value,
            Ior<Error, A>.Both both => BindIorBoth(both),
            _ => throw new InvalidOperationException()
        };

    private A BindIorBoth<A>(Ior<Error, A>.Both both)
    {
        _ = ((IAccumulate<Error>)this).Accumulate(both.LeftValue);
        return both.RightValue;
    }
}

internal sealed class RaiseNelAdapter<Error>(IAccumulate<Error> accumulate) : IRaise<NonEmptyList<Error>>
{
    [DoesNotReturn]
    public void Raise(NonEmptyList<Error> error)
    {
        _ = accumulate.AccumulateAll(error);
        throw AccumulatingStoppedException.Instance;
    }
}

internal sealed class ListAccumulate<Error>(IRaise<NonEmptyList<Error>> raise) :
    IAccumulate<Error>,
    IRaise<NonEmptyList<Error>>
{
    private readonly List<Error> _errors = [];

    private AccumulateError? _cachedError;

    [DoesNotReturn]
    public void Raise(NonEmptyList<Error> errors) =>
        raise.Raise(NonEmptyList<Error>.FromList(_errors.Concat(errors.All).ToList()));

    [DoesNotReturn]
    internal Exception RaiseSingle(Error error)
    {
        Raise(error.Nel());
        throw new InvalidOperationException("Unreachable");
    }

    public AccumulateValue AccumulateAll(NonEmptyList<Error> errors)
    {
        _errors.AddRange(errors.All);
        return CachedError;
    }

    public AccumulateError? LatestError => _errors.Count > 0 ? CachedError : null;

    private AccumulateError CachedError =>
        _cachedError ??= new AccumulateError(RaiseAllCached);

    [DoesNotReturn]
    private Exception RaiseAllCached()
    {
        raise.Raise(NonEmptyList<Error>.FromList(_errors.ToList()));
        throw new InvalidOperationException("Unreachable");
    }
}

internal sealed class TolerantAccumulate<Error>(
    IAccumulate<Error> underlying,
    IRaise<AccumulateValue> raise) : IAccumulate<Error>
{
    public AccumulateValue AccumulateAll(NonEmptyList<Error> errors)
    {
        _ = underlying.AccumulateAll(errors);
        return new AccumulateError(RaiseInterrupt);
    }

    public AccumulateError? LatestError =>
        underlying.LatestError is null ? null : new AccumulateError(RaiseInterrupt);

    [DoesNotReturn]
    private Exception RaiseInterrupt()
    {
        raise.Raise(new AccumulateError(RaiseInterrupt));
        throw new InvalidOperationException("Unreachable");
    }
}

internal static class AccumulateSupport
{
    internal static IAccumulate<Error> Tolerant<Error>(
        this IAccumulate<Error> underlying,
        IRaise<AccumulateValue> raise) =>
        new TolerantAccumulate<Error>(underlying, raise);
}

internal static class AccumulateValueExtensions
{
    internal static A GetValue<A>(this AccumulateValue value)
    {
        if (value is AccumulateOk<A> ok)
            return ok.Value;
        _ = ((AccumulateError)value).GetValue();
        throw new InvalidOperationException("Unreachable");
    }
}
