using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Arrow.Core.Atomic;

namespace Arrow.Core.Raise;

public static class RaiseBuilders
{
    
    public static A Singleton<A>(Func<A> onRaise, Func<SingletonRaise, A> block) =>
        RaiseExtensions.Recover<Unit, A>(
            r => block(new SingletonRaise(new UnitRaiseAdapter(r))),
            _ => onRaise());

    public static Either<Error, A> RunEither<Error, A>(Func<IRaise<Error>, A> block) =>
        RaiseExtensions.FoldOrThrow<Error, A, Either<Error, A>>(
            block,
            static e => new Either<Error, A>.Left(e),
            static a => new Either<Error, A>.Right(a));

    public static A? RunNullable<A>(Func<SingletonRaise, A> block) =>
        typeof(A).IsValueType
            ? (A?)NullableStructRaise.RunUntyped(typeof(A), block)!
            : (A?)NullableReferenceRaise.RunUntyped(typeof(A), block)!;

    public static OperationResult<A> RunResult<A>(Func<ResultRaise, A> block) =>
        RaiseExtensions.Fold<Exception, A, OperationResult<A>>(
            r => block(new ResultRaise(r)),
            static ex => OperationResult<A>.Failure(ex),
            static ex => OperationResult<A>.Failure(ex),
            static a => OperationResult<A>.Success(a));

    public static Option<A> RunOption<A>(Func<SingletonRaise, A> block) =>
        Singleton(
            OptionExtensions.None<A>,
            r => OptionExtensions.Some(block(r)));

    public static Ior<Error, A> RunIor<Error, A>(
        Func<Error, Error, Error> combineError,
        Func<IorRaise<Error>, A> block)
    {
        var state = new Atomic<object?>(EmptyValue.Sentinel);
        return RaiseExtensions.FoldOrThrow<Error, A, Ior<Error, A>>(
            r => block(new IorRaise<Error>(combineError, state, r)),
            e => (Ior<Error, A>)new Ior<Error, A>.Left(
                EmptyValue.Combine(state.Get(), e, combineError)),
            a => EmptyValue.Fold<Error, Ior<Error, A>>(
                state.Get(),
                () => new Ior<Error, A>.Right(a),
                e => new Ior<Error, A>.Both(e, a)));
    }

    public static Ior<NonEmptyList<Error>, A> RunIorNel<Error, A>(
        Func<NonEmptyList<Error>, NonEmptyList<Error>, NonEmptyList<Error>>? combineError,
        Func<IorRaise<NonEmptyList<Error>>, A> block) =>
        RunIor(combineError ?? CombineNel, block);

    public static void Impure(Action<SingletonRaise> block) =>
        Singleton(static () => Unit.Value, r => { block(r); return Unit.Value; });

    private static NonEmptyList<Error> CombineNel<Error>(
        NonEmptyList<Error> a,
        NonEmptyList<Error> b) =>
        NonEmptyList<Error>.FromList(a.All.Concat(b.All).ToList());
}

internal static class NullableStructRaise
{
    internal static A? Run<A>(Func<SingletonRaise, A> block) where A : struct =>
        RaiseExtensions.FoldOrThrow<Unit, A, A?>(
            r => block(new SingletonRaise(new UnitRaiseAdapter(r))),
            static _ => null,
            static a => a);

    internal static object? RunUntyped(Type type, object block) =>
        RunUntypedMethod.MakeGenericMethod(type).Invoke(null, [block]);

    private static A? RunUntypedImpl<A>(Func<SingletonRaise, A> block) where A : struct =>
        Run(block);

    private static readonly MethodInfo RunUntypedMethod =
        typeof(NullableStructRaise).GetMethod(
            nameof(RunUntypedImpl),
            BindingFlags.Static | BindingFlags.NonPublic)!;

    internal static A? Null<A>() where A : struct => null;
}

internal static class NullableReferenceRaise
{
    internal static A? Run<A>(Func<SingletonRaise, A> block) where A : class =>
        RaiseExtensions.FoldOrThrow<Unit, A, A?>(
            r => block(new SingletonRaise(new UnitRaiseAdapter(r))),
            static _ => null,
            static a => a);

    internal static object? RunUntyped(Type type, object block) =>
        RunUntypedMethod.MakeGenericMethod(type).Invoke(null, [block]);

    private static A? RunUntypedImpl<A>(Func<SingletonRaise, A> block) where A : class =>
        Run(block);

    private static readonly MethodInfo RunUntypedMethod =
        typeof(NullableReferenceRaise).GetMethod(
            nameof(RunUntypedImpl),
            BindingFlags.Static | BindingFlags.NonPublic)!;

    internal static A? Null<A>() where A : class => null;
}

public sealed class SingletonRaise(UnitRaiseAdapter inner)
{
    
    [DoesNotReturn]
    public void Raise() => inner.RaiseUnit();

    
    public void Ensure(bool condition)
    {
        if (!condition)
            Raise();
    }

    
    public A Bind<A>(Option<A> option) =>
        option switch
        {
            Option<A>.Some some => some.Value,
            _ => throw RaiseNone()
        };

    
    public A BindNullable<A>(A? value) where A : class =>
        value ?? throw RaiseNone();

    
    public A EnsureNotNull<A>(A? value) where A : class =>
        value ?? throw RaiseNone();

    
    public Dictionary<K, V> BindAllNullable<K, V>(IReadOnlyDictionary<K, V?> map) where K : notnull where V : class =>
        map.ToDictionary(static kv => kv.Key, kv => BindNullable(kv.Value));

    
    public Dictionary<K, V> BindAllOption<K, V>(IReadOnlyDictionary<K, Option<V>> map) where K : notnull =>
        map.ToDictionary(static kv => kv.Key, kv => Bind(kv.Value));

    
    public List<A> BindAllNullable<A>(IEnumerable<A?> values) where A : class =>
        values.Select(BindNullable).ToList();

    
    public List<A> BindAllOption<A>(IEnumerable<Option<A>> values) =>
        values.Select(Bind).ToList();

    
    public NonEmptyList<A> BindAllNullable<A>(NonEmptyList<A?> values) where A : class =>
        NonEmptyList<A>.FromList(values.All.Select(BindNullable).ToList());

    
    public NonEmptyList<A> BindAllOption<A>(NonEmptyList<Option<A>> values) =>
        NonEmptyList<A>.FromList(values.All.Select(Bind).ToList());

    
    public NonEmptySet<A> BindAllNullable<A>(NonEmptySet<A?> values) where A : class =>
        NonEmptySet<A>.FromSet(values.Elements.Select(BindNullable).ToHashSet());

    
    public NonEmptySet<A> BindAllOption<A>(NonEmptySet<Option<A>> values) =>
        NonEmptySet<A>.FromSet(values.Elements.Select(Bind).ToHashSet());

    
    public A Recover<A>(Func<SingletonRaise, A> block, Func<A> onRaise) =>
        RaiseExtensions.Recover<Unit, A>(
            r => block(new SingletonRaise(new UnitRaiseAdapter(r))),
            _ => onRaise());

    [DoesNotReturn]
    private RaiseCancellationException RaiseNone()
    {
        Raise();
        throw new InvalidOperationException("Unreachable");
    }
}

public sealed class UnitRaiseAdapter(IRaise<Unit> inner)
{
    [DoesNotReturn]
    public void RaiseUnit() => inner.Raise(Unit.Value);
}

public sealed class ResultRaise(IRaise<Exception> inner) : IRaise<Exception>
{
    [DoesNotReturn]
    public void Raise(Exception error) => inner.Raise(error);

    
    public A Bind<A>(OperationResult<A> result) =>
        result.IsSuccess ? result.Value : throw RaiseResult(result.Exception!);

    
    public Dictionary<K, V> BindAllResult<K, V>(IReadOnlyDictionary<K, OperationResult<V>> map) where K : notnull =>
        map.ToDictionary(static kv => kv.Key, kv => Bind(kv.Value));

    
    public List<A> BindAllResult<A>(IEnumerable<OperationResult<A>> values) =>
        values.Select(Bind).ToList();

    
    public NonEmptyList<A> BindAllResult<A>(NonEmptyList<OperationResult<A>> values) =>
        NonEmptyList<A>.FromList(values.All.Select(Bind).ToList());

    
    public NonEmptySet<A> BindAllResult<A>(NonEmptySet<OperationResult<A>> values) =>
        NonEmptySet<A>.FromSet(values.Elements.Select(Bind).ToHashSet());

    
    public A Recover<A>(Func<ResultRaise, A> block, Func<Exception, A> recover) =>
        RaiseExtensions.Recover<Exception, A>(
            r => block(new ResultRaise(r)),
            recover);

    [DoesNotReturn]
    private RaiseCancellationException RaiseResult(Exception ex)
    {
        Raise(ex);
        throw new InvalidOperationException("Unreachable");
    }
}

public sealed class IorRaise<Error>(
    Func<Error, Error, Error> combineError,
    Atomic<object?> state,
    IRaise<Error> inner) : IRaise<Error>
{
    [DoesNotReturn]
    public void Raise(Error error) => inner.Raise(error);

    
    public void Accumulate(Error value) => Bind(new Ior<Error, Unit>.Both(value, Unit.Value));

    
    public A GetOrAccumulate<A>(Either<Error, A> either, Func<Error, A> recover) =>
        Bind(either.Fold(
            e => (Ior<Error, A>)new Ior<Error, A>.Both(e, recover(e)),
            static a => new Ior<Error, A>.Right(a)));

    
    public A Bind<A>(Ior<Error, A> ior) =>
        ior switch
        {
            Ior<Error, A>.Left left => throw RaiseIorError(left.Value),
            Ior<Error, A>.Right right => right.Value,
            Ior<Error, A>.Both both => BindBoth(both),
            _ => throw new InvalidOperationException()
        };

    
    public List<A> BindAllIor<A>(IEnumerable<Ior<Error, A>> values) =>
        values.Select(Bind).ToList();

    
    public List<A> BindAllIor<A>(NonEmptyList<Ior<Error, A>> values) =>
        values.All.Select(Bind).ToList();

    
    public List<A> BindAllIor<A>(NonEmptySet<Ior<Error, A>> values) =>
        values.Elements.Select(Bind).ToList();

    
    public Dictionary<K, V> BindAllIor<K, V>(IReadOnlyDictionary<K, Ior<Error, V>> map) where K : notnull =>
        map.ToDictionary(kv => kv.Key, kv => Bind(kv.Value));

    
    public A Recover<A>(Func<IorRaise<Error>, A> block, Func<Error, A> recover)
    {
        var nestedState = new Atomic<object?>(EmptyValue.Sentinel);
        return RaiseExtensions.Recover<Error, A>(
            r =>
            {
                try
                {
                    return block(new IorRaise<Error>(combineError, nestedState, r));
                }
                finally
                {
                    var accumulated = nestedState.Get();
                    if (!ReferenceEquals(accumulated, EmptyValue.Sentinel))
                        state.Set(EmptyValue.Combine(state.Get(), (Error)accumulated!, combineError));
                }
            },
            recover);
    }

    private A BindBoth<A>(Ior<Error, A>.Both both)
    {
        Combine(both.LeftValue);
        return both.RightValue;
    }

    private void Combine(Error error) =>
        state.Set(EmptyValue.Combine(state.Get(), error, combineError));

    [DoesNotReturn]
    private RaiseCancellationException RaiseIorError(Error error)
    {
        Raise(error);
        throw new InvalidOperationException("Unreachable");
    }
}
