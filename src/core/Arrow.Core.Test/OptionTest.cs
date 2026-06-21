using Arrow.Core;
using Arrow.Core.Raise;
using Arrow.Core.Test.Generators;
using Arrow.Core.Test.Laws;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class OptionTest
{
    private static readonly Option<string> Some = OptionExtensions.Some("kotlin");
    private static readonly Option<string> None = OptionExtensions.None<string>();

    [Fact]
    public void MonoidLaws() =>
        LawTesting.TestLaws(
            new MonoidLaws<Option<int>>(
                "Option",
                OptionExtensions.None<int>(),
                static (x, y) => x.Combine(y, static (a, b) => a + b),
                Arb.From(ArrowGenerators.GenOptionStruct(Arb.Default.Int32().Generator))));

    [Property]
    public void EnsureNullInOptionComputation(bool predicate, int i)
    {
        var actual = RaiseBuilders.RunOption(r =>
        {
            r.Ensure(predicate);
            return i;
        });
        var expected = predicate ? OptionExtensions.Some(i) : OptionExtensions.None<int>();
        Assert.Equal(expected, actual);
    }

    [Property]
    public void EnsureNotNullInOptionComputation(int? i)
    {
        static int Square(int value) => value * value;

        var actual = RaiseBuilders.RunOption(r => Square(r.Bind(i is null ? OptionExtensions.None<int>() : OptionExtensions.Some(i.Value))));
        var expected = i is null ? OptionExtensions.None<int>() : OptionExtensions.Some(Square(i.Value));
        LawTesting.EqualUnderLaw(expected, actual);
    }

    [Fact]
    public void ShortCircuitNull()
    {
        var actual = RaiseBuilders.RunOption<int>(r =>
        {
            var number = "s".Length;
            r.Bind(number > 1 ? OptionExtensions.Some(number) : OptionExtensions.None<int>());
            throw new InvalidOperationException("This should not be executed");
        });
        Assert.Equal(OptionExtensions.None<int>(), actual);
    }

    [Property]
    public void TapAppliesEffectsReturningTheOriginalValue(Option<long> option)
    {
        var effect = 0;
        var res = option.OnSome(_ => effect += 1);
        var expected = option.IsSome() ? 1 : 0;
        Assert.Equal(expected, effect);
        Assert.Equal(option, res);
    }

    [Property]
    public void TapNoneAppliesEffectsReturningTheOriginalValue(Option<long> option)
    {
        var effect = 0;
        var res = option.OnNone(() => effect += 1);
        var expected = option.IsNone() ? 1 : 0;
        Assert.Equal(expected, effect);
        Assert.Equal(option, res);
    }

    [Property]
    public void FromNullableShouldWorkForBothNullAndNonNullValuesOfNullableTypes(int? a)
    {
        var o = a is null ? OptionExtensions.None<int>() : OptionExtensions.Some(a.Value);
        if (a is null)
            LawTesting.EqualUnderLaw(OptionExtensions.None<int>(), o);
        else
            LawTesting.EqualUnderLaw(OptionExtensions.Some(a.Value), o);
    }

    [Fact]
    public void FromNullableShouldReturnNoneForNullValuesOfNullableTypes()
    {
        int? a = null;
        LawTesting.EqualUnderLaw(OptionExtensions.None<int>(), a is null ? OptionExtensions.None<int>() : OptionExtensions.Some(a.Value));
    }

    [Fact]
    public void GetOrElse()
    {
        Assert.Equal("kotlin", Some.GetOrElse(static () => "java"));
        Assert.Equal("java", None.GetOrElse(static () => "java"));
    }

    [Fact]
    public void GetOrNull()
    {
        Assert.NotNull(Some.GetOrNull());
        Assert.Null(None.GetOrNull());
    }

    [Fact]
    public void Map()
    {
        Assert.Equal(OptionExtensions.Some("KOTLIN"), Some.Map(static s => s.ToUpperInvariant()));
        Assert.Equal(OptionExtensions.None<string>(), None.Map(static s => s.ToUpperInvariant()));
    }

    [Fact]
    public void Fold()
    {
        Assert.Equal(6, Some.Fold(static () => 0, static s => s.Length));
        Assert.Equal(0, None.Fold(static () => 0, static s => s.Length));
    }

    [Fact]
    public void FlatMap()
    {
        Assert.Equal(OptionExtensions.Some("KOTLIN"), Some.FlatMap(static s => OptionExtensions.Some(s.ToUpperInvariant())));
        Assert.Equal(OptionExtensions.None<string>(), None.FlatMap(static s => OptionExtensions.Some(s.ToUpperInvariant())));
    }

    [Fact]
    public void Filter()
    {
        Assert.Equal(OptionExtensions.None<string>(), Some.Filter(static s => s == "java"));
        Assert.Equal(OptionExtensions.None<string>(), None.Filter(static s => s == "java"));
        Assert.Equal(Some, Some.Filter(static s => s.StartsWith('k')));
    }

    [Fact]
    public void FilterNot()
    {
        Assert.Equal(Some, Some.FilterNot(static s => s == "java"));
        Assert.Equal(OptionExtensions.None<string>(), None.FilterNot(static s => s == "java"));
        Assert.Equal(OptionExtensions.None<string>(), Some.FilterNot(static s => s.StartsWith('k')));
    }

    [Fact]
    public void FilterIsInstance()
    {
        Assert.Equal(Some, OptionExtensions.FilterIsInstance<string, string>(Some));
        Assert.Equal(OptionExtensions.None<int>(), OptionExtensions.FilterIsInstance<string, int>(Some));
        Assert.Equal(OptionExtensions.None<string>(), OptionExtensions.FilterIsInstance<string, string>(None));
        Assert.Equal(OptionExtensions.None<int>(), OptionExtensions.FilterIsInstance<string, int>(None));
    }

    [Fact]
    public void ToList()
    {
        Assert.Equal(new[] { "kotlin" }, Some.ToList());
        Assert.Equal(Array.Empty<string>(), None.ToList());
    }

    [Fact]
    public void IterableFirstOrNone()
    {
        var iterable = new[] { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(1), iterable.FirstOrNone());
        Assert.Equal(OptionExtensions.Some(3), iterable.FirstOrNone(static i => i > 2));
        Assert.Equal(OptionExtensions.None<int>(), iterable.FirstOrNone(static i => i > 7));
        Assert.Equal(OptionExtensions.None<int>(), Array.Empty<int>().FirstOrNone());

        int?[] nullableIterable1 = [null, 2, 3, 4, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableIterable1.FirstOrNone());

        int?[] nullableIterable2 = [1, 2, 3, null, 5, null];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableIterable2.FirstOrNone(static i => i is null));
    }

    [Fact]
    public void CollectionFirstOrNone()
    {
        var list = new List<int> { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(1), list.FirstOrNone());
        Assert.Equal(OptionExtensions.None<int>(), new List<int>().FirstOrNone());

        int?[] nullableList = [null, 2, 3, 4, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableList.FirstOrNone());
    }

    [Fact]
    public void IterableSingleOrNone()
    {
        var iterable = new[] { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.None<int>(), iterable.SingleOrNone());
        Assert.Equal(OptionExtensions.None<int>(), iterable.SingleOrNone(static i => i > 2));

        var singleIterable = new[] { 3 };
        Assert.Equal(OptionExtensions.Some(3), singleIterable.SingleOrNone());
        Assert.Equal(OptionExtensions.Some(3), singleIterable.SingleOrNone(static i => i == 3));

        int?[] nullableSingleIterable1 = [null];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableSingleIterable1.SingleOrNone());

        int?[] nullableSingleIterable2 = [1, 2, 3, null, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableSingleIterable2.SingleOrNone(static i => i is null));

        int?[] nullableSingleIterable3 = [1, 2, 3, null, 5, null];
        Assert.Equal(OptionExtensions.None<int?>(), nullableSingleIterable3.SingleOrNone(static i => i is null));
    }

    [Fact]
    public void CollectionSingleOrNone()
    {
        var list = new List<int> { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.None<int>(), list.SingleOrNone());

        var singleList = new List<int> { 3 };
        Assert.Equal(OptionExtensions.Some(3), singleList.SingleOrNone());

        int?[] nullableSingleList = [null];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableSingleList.SingleOrNone());
    }

    [Fact]
    public void IterableLastOrNone()
    {
        var iterable = new[] { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(6), iterable.LastOrNone());
        Assert.Equal(OptionExtensions.Some(3), iterable.LastOrNone(static i => i < 4));
        Assert.Equal(OptionExtensions.None<int>(), iterable.LastOrNone(static i => i > 7));
        Assert.Equal(OptionExtensions.None<int>(), Array.Empty<int>().LastOrNone());

        int?[] nullableIterable1 = [1, 2, 3, 4, 5, null];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableIterable1.LastOrNone());

        int?[] nullableIterable2 = [null, 2, 3, null, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableIterable2.LastOrNone(static i => i is null));
    }

    [Fact]
    public void CollectionLastOrNone()
    {
        var list = new List<int> { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(6), list.LastOrNone());
        Assert.Equal(OptionExtensions.None<int>(), new List<int>().LastOrNone());

        int?[] nullableList = [1, 2, 3, 4, 5, null];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableList.LastOrNone());
    }

    [Fact]
    public void IterableElementAtOrNone()
    {
        var iterable = new[] { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(3), iterable.ElementAtOrNone(3 - 1));
        Assert.Equal(OptionExtensions.None<int>(), iterable.ElementAtOrNone(-1));
        Assert.Equal(OptionExtensions.None<int>(), iterable.ElementAtOrNone(100));

        int?[] nullableIterable = [1, 2, null, 4, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableIterable.ElementAtOrNone(3 - 1));
    }

    [Fact]
    public void CollectionElementAtOrNone()
    {
        var list = new List<int> { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(OptionExtensions.Some(3), list.ElementAtOrNone(3 - 1));
        Assert.Equal(OptionExtensions.None<int>(), list.ElementAtOrNone(-1));
        Assert.Equal(OptionExtensions.None<int>(), list.ElementAtOrNone(100));

        int?[] nullableList = [1, 2, null, 4, 5, 6];
        Assert.Equal(OptionExtensions.Some<int?>(null), nullableList.ElementAtOrNone(3 - 1));
    }

    [Fact]
    public void ToLeftOption()
    {
        Assert.Equal(1, 1.LeftIor().LeftOrNull());
        Assert.Null(2.RightIor().LeftOrNull());
        Assert.Equal(1, (1, 2).BothIor().LeftOrNull());
    }

    [Fact]
    public void OptionPairToMap()
    {
        var some = OptionExtensions.Some(("key", "value"));
        var none = OptionExtensions.None<(string, string)>();
        Assert.Equal(new Dictionary<string, string> { ["key"] = "value" }, some.ToMap());
        Assert.Equal(new Dictionary<string, string>(), none.ToMap());
    }

    [Fact]
    public void CatchShouldReturnSomeResultWhenFDoesNotThrow()
    {
        Assert.Equal(
            OptionExtensions.Some(1),
            OptionCatch(static _ => 1, static _ => OptionExtensions.None<int>()));
    }

    [Fact]
    public void CatchWithDefaultRecoverShouldReturnSomeResultWhenFDoesNotThrow()
    {
        Assert.Equal(
            OptionExtensions.Some(1),
            OptionCatch(static _ => 1));
    }

    [Fact]
    public void CatchShouldReturnSomeRecoverValueWhenFThrows()
    {
        var exception = new Exception("Boom!");
        const int recoverValue = 10;
        Assert.Equal(
            OptionExtensions.Some(recoverValue),
            OptionCatch(
                _ => throw exception,
                _ => OptionExtensions.Some(recoverValue)));
    }

    [Fact]
    public void CatchShouldReturnNoneWhenFThrows()
    {
        var exception = new Exception("Boom!");
        Assert.Equal(
            OptionExtensions.None<int>(),
            OptionCatch<int>(_ => throw exception));
    }

    [Property]
    public void InvokeOperatorShouldReturnSome(int a) =>
        Assert.Equal(OptionExtensions.Some(a), Option<int>.Invoke(a));

    [Fact]
    public void IsNoneShouldReturnTrueIfNoneAndFalseIfSome()
    {
        Assert.True(None.IsNone());
        Assert.False(None.IsSome());
    }

    [Fact]
    public void IsSomeShouldReturnTrueIfSomeAndFalseIfNone()
    {
        Assert.True(Some.IsSome());
        Assert.False(Some.IsNone());
    }

    [Fact]
    public void IsSomeWithPredicate()
    {
        Assert.True(Some.IsSome(static s => s.StartsWith('k')));
        Assert.False(Some.IsSome(static s => s.StartsWith('j')));
        Assert.False(None.IsSome(static s => s.StartsWith('k')));
    }

    [Property]
    public void Flatten(int a)
    {
        Assert.Equal(OptionExtensions.Some(a), OptionExtensions.Some(OptionExtensions.Some(a)).Flatten());
        Assert.Equal(OptionExtensions.None<int>(), OptionExtensions.Some(OptionExtensions.None<int>()).Flatten());
    }

    [Property]
    public void CompareToWithSomeValues(int a, int b)
    {
        var opA = Option<int>.Invoke(a);
        var opB = Option<int>.Invoke(b);
        Assert.Equal(a > b, opA.CompareTo(opB) > 0);
        Assert.Equal(a >= b, opA.CompareTo(opB) >= 0);
        Assert.Equal(a < b, opA.CompareTo(opB) < 0);
        Assert.Equal(a <= b, opA.CompareTo(opB) <= 0);
        Assert.Equal(a == b, opA.Equals(opB));
        Assert.Equal(a != b, !opA.Equals(opB));
    }

    [Fact]
    public void CompareToWithNoneValues()
    {
        var opA = Option<int>.Invoke(1);
        var opB = OptionExtensions.None<int>();
        Assert.True(opA.CompareTo(opB) > 0);
        Assert.True(opA.CompareTo(opB) >= 0);
        Assert.False(opA.CompareTo(opB) < 0);
        Assert.False(opA.CompareTo(opB) <= 0);
        Assert.NotEqual(opA, opB);
        Assert.NotEqual(opB, opA);

        Assert.False(None.CompareTo(Some) > 0);
        Assert.False(None.CompareTo(Some) >= 0);
        Assert.True(None.CompareTo(Some) < 0);
        Assert.True(None.CompareTo(Some) <= 0);
        Assert.NotEqual(None, Some);
        Assert.NotEqual(Some, None);
    }

    [Property]
    public void Recover(Option<int> op, int a)
    {
        var expected = op.Fold(
            () => a.ToOption(),
            _ => op);
        Assert.Equal(expected, OptionRecover(op, () => a));
    }

    [Property]
    public void ToEither(Option<int> op, int a)
    {
        var expected = op.Fold<Either<int, int>>(
            () => EitherExtensions.Left<int, int>(a),
            EitherExtensions.Right<int, int>);
        LawTesting.EqualUnderLaw(expected, op.ToEither(() => a));
    }

    private static Option<A> OptionCatch<A>(Func<SingletonRaise, A> block, Func<Exception, Option<A>>? recover = null) =>
        RaiseExtensions.Catch(
            () => RaiseBuilders.RunOption(block),
            recover ?? DefaultRecover<A>);

    private static Option<A> DefaultRecover<A>(Exception _) => OptionExtensions.None<A>();

    private static Option<A> OptionRecover<A>(Option<A> option, Func<A> recover) =>
        option switch
        {
            Option<A>.None => RaiseBuilders.RunOption(_ => recover()),
            _ => option
        };
}
