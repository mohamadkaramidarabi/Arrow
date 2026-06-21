using FsCheck;

namespace Arrow.Core.Test.Generators;

public static class ArrowGenerators
{
    private static int _functionSeed;

    public static Gen<NonEmptyList<T>> GenNonEmptyList<T>(Gen<T> element, int min = 1, int max = 100) =>
        SizedList(element, min, max).Where(static l => l.Count > 0)
            .Select(static l => NonEmptyList<T>.Of(l));

    public static Gen<NonEmptySet<T>> GenNonEmptySet<T>(Gen<T> element, int min = 1, int max = 100) =>
        SizedList(element, min, max).Select(static l => l.ToHashSet())
            .Where(static s => s.Count > 0)
            .Select(static s => NonEmptySet<T>.Of(s));

    public static Gen<IEnumerable<T>> GenSequence<T>(Gen<T> element, int min = 0, int max = 100) =>
        SizedList(element, min, max).Select(static l => l.AsEnumerable());

    public static Gen<int> IntSmall(int factor = 10_000) =>
        Gen.Choose(int.MinValue / factor, int.MaxValue / factor);

    public static Gen<long> LongSmall() =>
        Gen.Choose(int.MinValue / 100, int.MaxValue / 100).Select(static i => (long)i * 1000L);

    public static Gen<Option<T>> GenOption<T>(Gen<T> element) where T : class =>
        element.OrNull().Select(static v => v.ToOption());

    public static Gen<Option<T>> GenOptionStruct<T>(Gen<T> element) where T : struct =>
        Gen.OneOf(
            element.Select(static v => OptionExtensions.Some(v)),
            Gen.Constant(OptionExtensions.None<T>()));

    public static Gen<Either<NonEmptyList<E>, A>> GenEitherNel<E, A>(Gen<E> elementGen, Gen<A> genA) =>
        GenEither(GenNonEmptyList(elementGen), genA);

    public static Gen<Either<E, A>> GenEither<E, A>(Gen<E> genE, Gen<A> genA) =>
        Gen.OneOf(
            genE.Select(static e => (Either<E, A>)EitherExtensions.Left<E, A>(e)),
            genA.Select(static a => (Either<E, A>)EitherExtensions.Right<E, A>(a)));

    public static Gen<Ior<A, B>> GenIor<A, B>(Gen<A> genA, Gen<B> genB) =>
        Gen.OneOf(
            genA.Select(static a => (Ior<A, B>)new Ior<A, B>.Left(a)),
            Gen.Zip(genA, genB).Select(static t => (Ior<A, B>)new Ior<A, B>.Both(t.Item1, t.Item2)),
            genB.Select(static b => (Ior<A, B>)new Ior<A, B>.Right(b)));

    public static Gen<OperationResult<T>> GenResult<T>(Gen<T> element) =>
        Gen.OneOf(
            element.Select(OperationResult<T>.Success),
            Gen.Elements(new Exception(), new InvalidOperationException(), new ArgumentException())
                .Select(OperationResult<T>.Failure));

    public static Gen<object> Any() =>
        Gen.OneOf<object>(
            Arb.Default.String().Generator.Select(static s => (object)s),
            Arb.Default.Int32().Generator.Select(static i => (object)i),
            Arb.Default.Int64().Generator.Select(static l => (object)l),
            Arb.Default.Bool().Generator.Select(static b => (object)b),
            Gen.Elements(new Exception(), new InvalidOperationException()).Select(static e => (object)e),
            Gen.Constant(Unit.Value).Select(static u => (object)u));

    public static Gen<Func<A, B>> FunctionAToB<A, B>(Gen<B> element) where A : notnull =>
        Gen.Fresh<Func<A, B>>(() =>
        {
            var seed = Interlocked.Increment(ref _functionSeed);
            var value = element.Sample(seed, 1).First();
            var memo = MemoizedDeepRecursiveFunction<A, B>.Create((_, _) => value);
            return memo.Invoke;
        });

    public static Gen<Func<A, B, C, D>> FunctionABCToD<A, B, C, D>(Gen<D> element)
        where A : notnull where B : notnull where C : notnull =>
        Gen.Fresh<Func<A, B, C, D>>(() =>
        {
            var seed = Interlocked.Increment(ref _functionSeed);
            var value = element.Sample(seed, 1).First();
            var memo = MemoizedDeepRecursiveFunction<(A, B, C), D>.Create((_, _) => value);
            return (a, b, c) => memo.Invoke((a, b, c));
        });

    public static Gen<(IReadOnlyDictionary<K, A>, IReadOnlyDictionary<K, B>)> Map2<K, A, B>(
        Gen<K> genK,
        Gen<A> genA,
        Gen<B> genB) where K : notnull =>
        DictionaryOf(genK, Value2(genA, genB), 30)
            .Select(map => DestructureMap2(map));

    private static Gen<List<T>> SizedList<T>(Gen<T> element, int min, int max) =>
        Gen.Choose(min, max).SelectMany(size => Gen.ListOf(size, element).Select(static l => l.ToList()));

    private static Gen<Dictionary<K, V>> DictionaryOf<K, V>(Gen<K> keyGen, Gen<V> valueGen, int maxSize)
        where K : notnull =>
        Gen.Choose(0, maxSize).SelectMany(size =>
            Gen.ListOf(size, Gen.Zip(keyGen, valueGen)).Select(entries =>
            {
                var dict = new Dictionary<K, V>();
                foreach (var entry in entries)
                {
                    var (key, value) = entry;
                    dict.TryAdd(key, value);
                }

                return dict;
            }));

    private static Gen<(Option<A>?, Option<B>?)> Value2<A, B>(Gen<A> genA, Gen<B> genB) =>
        Gen.OneOf(
            Gen.Zip(genA, genB).Select(static t =>
                ((Option<A>?)OptionExtensions.Some(t.Item1), (Option<B>?)OptionExtensions.Some(t.Item2))),
            genA.Select(static a => ((Option<A>?)OptionExtensions.Some(a), (Option<B>?)null)),
            genB.Select(static b => ((Option<A>?)null, (Option<B>?)OptionExtensions.Some(b))));

    private static (IReadOnlyDictionary<K, A>, IReadOnlyDictionary<K, B>) DestructureMap2<K, A, B>(
        Dictionary<K, (Option<A>?, Option<B>?)> map)
        where K : notnull
    {
        var first = new Dictionary<K, A>();
        var second = new Dictionary<K, B>();

        foreach (var (key, pair) in map)
        {
            var (left, right) = pair;
            if (left is not null)
                first[key] = left.GetOrNull()!;
            if (right is not null)
                second[key] = right.GetOrNull()!;
        }

        return (first, second);
    }
}
