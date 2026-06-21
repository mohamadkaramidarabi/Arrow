namespace Arrow.Core;

public abstract record Option<A>
{
    public sealed record Some(A Value) : Option<A>;

    public sealed record None : Option<A>;

    public static Option<A> FromNullable(A? value) => value is null ? new None() : new Some(value);

    public static Option<A> Invoke(A value) => new Option<A>.Some(value);

    public Option<A> OnNone(Action action)
    {
        if (IsNone())
            action();
        return this;
    }

    public Option<A> OnSome(Action<A> action)
    {
        if (IsSome() && this is Some some)
            action(some.Value);
        return this;
    }

    public bool IsNone() => this is None;

    public bool IsSome() => this is Some;

    public bool IsSome(Func<A, bool> predicate) =>
        this is Some some && predicate(some.Value);

    public A? GetOrNull() => this.GetOrElse(() => default!);

    public Option<B> Map<B>(Func<A, B> f) => FlatMap(a => new Option<B>.Some(f(a)));

    public R Fold<R>(Func<R> ifEmpty, Func<A, R> ifSome) =>
        this switch
        {
            None => ifEmpty(),
            Some some => ifSome(some.Value),
            _ => throw new InvalidOperationException()
        };

    public Option<B> Bind<B>(Func<A, Option<B>> f) => FlatMap(f);

    public Option<B> FlatMap<B>(Func<A, Option<B>> f) =>
        this switch
        {
            None => OptionExtensions.None<B>(),
            Some some => f(some.Value),
            _ => throw new InvalidOperationException()
        };

    public Option<A> Filter(Func<A, bool> predicate) =>
        this switch
        {
            None => this,
            Some some => predicate(some.Value) ? some : new None(),
            _ => throw new InvalidOperationException()
        };

    public Option<A> FilterNot(Func<A, bool> predicate) =>
        this switch
        {
            None => this,
            Some some => !predicate(some.Value) ? some : new None(),
            _ => throw new InvalidOperationException()
        };

    public Either<L, A> ToEither<L>(Func<L> ifEmpty) =>
        this switch
        {
            None => new Either<L, A>.Left(ifEmpty()),
            Some some => new Either<L, A>.Right(some.Value),
            _ => throw new InvalidOperationException()
        };

    public IReadOnlyList<A> ToList() => Fold(Array.Empty<A>, static a => new[] { a });

    public override string ToString() =>
        Fold(static () => "Option.None", static a => $"Option.Some({a})");
}

public static class OptionExtensions
{
    public static Option<A> Some<A>(A value) => new Option<A>.Some(value);

    public static Option<A> None<A>() => new Option<A>.None();

    public static A GetOrElse<A>(this Option<A> option, Func<A> defaultValue) =>
        option switch
        {
            Option<A>.Some some => some.Value,
            _ => defaultValue()
        };

    public static Option<A> ToOption<A>(this A? value) =>
        value is null ? new Option<A>.None() : new Option<A>.Some(value);

    public static Option<B> FilterIsInstance<A, B>(this Option<A> option) where A : class =>
        option switch
        {
            Option<A>.None => new Option<B>.None(),
            Option<A>.Some some when some.Value is B b => new Option<B>.Some(b),
            Option<A>.Some => new Option<B>.None(),
            _ => throw new InvalidOperationException()
        };

    public static Option<A> Flatten<A>(this Option<Option<A>> option) =>
        option.FlatMap(Predef.Identity);

    public static IReadOnlyDictionary<K, V> ToMap<K, V>(this Option<(K Key, V Value)> option) where K : notnull =>
        option.ToList().ToDictionary(static p => p.Key, static p => p.Value);

    public static Option<A> Combine<A>(this Option<A> option, Option<A> other, Func<A, A, A> combine) =>
        option switch
        {
            Option<A>.Some left => other switch
            {
                Option<A>.Some right => new Option<A>.Some(combine(left.Value, right.Value)),
                Option<A>.None => option,
                _ => throw new InvalidOperationException()
            },
            Option<A>.None => other,
            _ => throw new InvalidOperationException()
        };

    public static int CompareTo<A>(this Option<A> option, Option<A> other) where A : IComparable<A> =>
        option.Fold(
            () => other.Fold(static () => 0, static _ => -1),
            a1 => other.Fold(static () => 1, a2 => a1.CompareTo(a2)));
}
