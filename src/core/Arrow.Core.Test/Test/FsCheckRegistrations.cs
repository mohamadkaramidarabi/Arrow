using System.Runtime.CompilerServices;
using Arrow.Core;
using Arrow.Core.Test.Generators;
using FsCheck;

namespace Arrow.Core.Test;

public sealed class FsCheckRegistrations
{
    [ModuleInitializer]
    internal static void Initialize() => RegisterAll();

    internal static void RegisterAll() => Arb.Register(typeof(FsCheckRegistrations));

    public static Arbitrary<Either<string, short>> EitherStringShort() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Int16().Generator));

    public static Arbitrary<Either<string, byte>> EitherStringByte() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Byte().Generator));

    public static Arbitrary<Either<string, int>> EitherStringInt() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<Either<string, long>> EitherStringLong() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Int64().Generator));

    public static Arbitrary<Either<string, float>> EitherStringFloat() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, FloatGen()));

    public static Arbitrary<Either<string, double>> EitherStringDouble() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, DoubleGen()));

    public static Arbitrary<Either<string, char>> EitherStringChar() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Char().Generator));

    public static Arbitrary<Either<string, string>> EitherStringString() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.String().Generator));

    public static Arbitrary<Either<string, bool>> EitherStringBool() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.String().Generator, BoolGen()));

    public static Arbitrary<Either<int, bool>> EitherIntBool() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.Int32().Generator, BoolGen()));

    public static Arbitrary<Either<bool, int>> EitherBoolInt() =>
        Arb.From(ArrowGenerators.GenEither(BoolGen(), Arb.Default.Int32().Generator));

    public static Arbitrary<Either<int, int>> EitherIntInt() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.Int32().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<Either<long, int>> EitherLongInt() =>
        Arb.From(ArrowGenerators.GenEither(Arb.Default.Int64().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, short>> EitherNelIntShort() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.Int16().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, byte>> EitherNelIntByte() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.Byte().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, int>> EitherNelIntInt() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, long>> EitherNelIntLong() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.Int64().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, float>> EitherNelIntFloat() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, FloatGen()));

    public static Arbitrary<Either<NonEmptyList<int>, double>> EitherNelIntDouble() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, DoubleGen()));

    public static Arbitrary<Either<NonEmptyList<int>, char>> EitherNelIntChar() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.Char().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, string>> EitherNelIntString() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, Arb.Default.String().Generator));

    public static Arbitrary<Either<NonEmptyList<int>, bool>> EitherNelIntBool() =>
        Arb.From(ArrowGenerators.GenEitherNel(Arb.Default.Int32().Generator, BoolGen()));

    public static Arbitrary<Either<E, A>> Either<E, A>(Arbitrary<E> left, Arbitrary<A> right) =>
        Arb.From(ArrowGenerators.GenEither(left.Generator, right.Generator));

    public static Arbitrary<Either<A, Either<A, B>>> EitherEither<A, B>(Arbitrary<A> left, Arbitrary<B> right) =>
        Arb.From(ArrowGenerators.GenEither(
            left.Generator,
            ArrowGenerators.GenEither(left.Generator, right.Generator)));

    public static Arbitrary<Either<string, Either<string, int>>> EitherStringEitherStringInt() =>
        Arb.From(ArrowGenerators.GenEither(
            Arb.Default.String().Generator,
            ArrowGenerators.GenEither(Arb.Default.String().Generator, Arb.Default.Int32().Generator)));

    public static Arbitrary<Either<string, Either<string, int>>> EitherStringEitherStringInt(
        Arbitrary<string> left,
        Arbitrary<int> right) =>
        EitherEither(left, right);

    public static Arbitrary<Ior<A, B>> Ior<A, B>(Arbitrary<A> left, Arbitrary<B> right) =>
        Arb.From(ArrowGenerators.GenIor(left.Generator, right.Generator));

    public static Arbitrary<Option<T>> Option<T>(Arbitrary<T> element) =>
        Arb.From(Gen.OneOf(
            element.Generator.Select(static x => (Option<T>)new Option<T>.Some(x)),
            Gen.Constant((Option<T>)new Option<T>.None())));

    public static Arbitrary<Option<int>> OptionInt(Arbitrary<int> element) => Option(element);

    public static Arbitrary<Option<int>> OptionInt() =>
        Arb.From(ArrowGenerators.GenOptionStruct(Arb.Default.Int32().Generator));

    public static Arbitrary<Option<long>> OptionLong(Arbitrary<long> element) => Option(element);

    public static Arbitrary<Option<long>> OptionLong() =>
        Arb.From(ArrowGenerators.GenOptionStruct(Arb.Default.Int64().Generator));

    public static Arbitrary<Ior<int, int>> IorIntInt() =>
        Arb.From(ArrowGenerators.GenIor(Arb.Default.Int32().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<Ior<string, int>> IorStringInt() =>
        Arb.From(ArrowGenerators.GenIor(Arb.Default.String().Generator, Arb.Default.Int32().Generator));

    public static Arbitrary<(int, int)> IntIntTuple() =>
        Arb.From(Gen.Zip(Arb.Default.Int32().Generator, Arb.Default.Int32().Generator)
            .Select(static t => (t.Item1, t.Item2)));

    public static Arbitrary<Dictionary<int, (int, int)>> DictionaryIntIntIntTuple() =>
        Arb.From(DictionaryOf(Arb.Default.Int32().Generator, IntIntTuple().Generator));

    public static Arbitrary<Dictionary<int, Ior<int, int>>> DictionaryIntIorIntInt() =>
        Arb.From(DictionaryOf(Arb.Default.Int32().Generator, IorIntInt().Generator));

    private static Gen<bool> BoolGen() => Arb.Default.Bool().Generator;

    private static Gen<float> FloatGen() => Gen.Choose(0, 10_000).Select(static i => i / 100f);

    private static Gen<double> DoubleGen() => Gen.Choose(0, 10_000).Select(static i => i / 100.0);

    private static Gen<Dictionary<K, V>> DictionaryOf<K, V>(Gen<K> keyGen, Gen<V> valueGen, int maxSize = 30)
        where K : notnull =>
        Gen.Choose(0, maxSize).SelectMany(size =>
            Gen.ListOf(size, Gen.Zip(keyGen, valueGen)).Select(entries =>
            {
                var dict = new Dictionary<K, V>();
                foreach (var (key, value) in entries)
                    dict.TryAdd(key, value);
                return dict;
            }));
}
