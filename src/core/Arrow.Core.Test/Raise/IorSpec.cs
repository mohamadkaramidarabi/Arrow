using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class IorSpec
{
    [Fact]
    public void Accumulates()
    {
        var result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
        {
            var one = r.Bind(new Ior<string, int>.Both("Hello", 1));
            var two = r.Bind(new Ior<string, int>.Both(", World!", 2));
            return one + two;
        });

        Assert.Equal(new Ior<string, int>.Both("Hello, World!", 3), result);
    }

    [Fact]
    public void AccumulatesAndShortCircuitsWithLeft()
    {
        var result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
        {
            var one = r.Bind(new Ior<string, int>.Both("Hello", 1));
            var two = r.Bind(new Ior<string, int>.Left(", World!"));
            return one + two;
        });

        Assert.Equal(new Ior<string, int>.Left("Hello, World!"), result);
    }

    [Fact]
    public void AccumulatesWithEither()
    {
        var result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
        {
            var one = r.Bind(new Ior<string, int>.Both("Hello", 1));
            var two = r.GetOrAccumulate(EitherExtensions.Left<string, int>(", World!"), _ => 0);
            return one + two;
        });

        Assert.Equal(new Ior<string, int>.Both("Hello, World!", 1), result);
    }

    [Fact]
    public async Task ConcurrentIorBind()
    {
        var result = RaiseBuilders.RunIor<HashSet<int>, List<int>>(
            (a, b) =>
            {
                var merged = new HashSet<int>(a);
                merged.UnionWith(b);
                return merged;
            },
            r =>
            {
                var tasks = Enumerable.Range(0, 5)
                    .Select(async i => r.Bind(new Ior<HashSet<int>, int>.Both(new HashSet<int> { i }, i)))
                    .ToArray();
                return Task.WhenAll(tasks).GetAwaiter().GetResult().ToList();
            });

        var both = Assert.IsType<Ior<HashSet<int>, List<int>>.Both>(result);
        Assert.Equal(new HashSet<int> { 0, 1, 2, 3, 4 }, both.LeftValue);
        Assert.Equal(new List<int> { 0, 1, 2, 3, 4 }, both.RightValue);
    }

    [Fact]
    public void AccumulatesAndShortCircuits()
    {
        var result = RaiseBuilders.RunIor<string, Unit>(string.Concat, r =>
        {
            _ = r.Bind(new Ior<string, Unit>.Both("Hello", Unit.Value));
            r.Raise(" World");
            return Unit.Value;
        });

        Assert.Equal(new Ior<string, Unit>.Left("Hello World"), result);
    }

    [Fact]
    public void IorRethrowsException()
    {
        var boom = new InvalidOperationException("Boom!");
        var thrown = Assert.Throws<InvalidOperationException>(() =>
            RaiseBuilders.RunIor<string, int>(string.Concat, _ => throw boom));
        Assert.Equal("Boom!", thrown.Message);
    }

    [Fact]
    public void RecoverWorksAsExpected()
    {
        var result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
        {
            var one = r.Recover(
                inner =>
                {
                    inner.Bind(new Ior<string, Unit>.Both("H", Unit.Value));
                    inner.Bind(new Ior<string, Unit>.Both("i", Unit.Value));
                    return inner.Bind(new Ior<string, int>.Left("Hello"));
                },
                _ => 1);
            var two = r.Bind(new Ior<string, int>.Right(2));
            var three = r.Bind(new Ior<string, int>.Both(", World", 3));
            return one + two + three;
        });

        Assert.Equal(new Ior<string, int>.Both("Hi, World", 6), result);
    }

    [Fact]
    public void RecoverWithRaiseIsNoOp()
    {
        var result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
        {
            var one = r.Recover(
                inner =>
                {
                    inner.Bind(new Ior<string, Unit>.Both("Hi", Unit.Value));
                    return inner.Bind(new Ior<string, int>.Left(", Hello"));
                },
                e =>
                {
                    r.Raise(e);
                    return 0;
                });
            var two = r.Bind(new Ior<string, int>.Right(2));
            var three = r.Bind(new Ior<string, int>.Both(", World", 3));
            return one + two + three;
        });

        Assert.Equal(new Ior<string, int>.Left("Hi, Hello"), result);
    }

    [Fact]
    public void IorNelAccumulates()
    {
        var result = RaiseBuilders.RunIorNel<string, int>(null, r =>
        {
            var one = r.Bind(new Ior<NonEmptyList<string>, int>.Both(NonEmptyList<string>.Of("ErrorOne"), 1));
            var two = r.Bind(new Ior<NonEmptyList<string>, int>.Both(NonEmptyList<string>.Of("ErrorTwo"), 2));
            return one + two;
        });

        var both = Assert.IsType<Ior<NonEmptyList<string>, int>.Both>(result);
        Assert.Equal(new List<string> { "ErrorOne", "ErrorTwo" }, both.LeftValue.All.ToList());
        Assert.Equal(3, both.RightValue);
    }

    [Fact]
    public void AccumulateErrorManually()
    {
        var result = RaiseBuilders.RunIor<string, string>(string.Concat, r =>
        {
            r.Accumulate("nonfatal");
            return "output";
        });

        Assert.Equal(new Ior<string, string>.Both("nonfatal", "output"), result);
    }

    [Fact]
    public void GetOrAccumulateEither()
    {
        var right = RaiseBuilders.RunIor<string, string>(string.Concat, r =>
        {
            var value = r.GetOrAccumulate(EitherExtensions.Right<string, string>("success"), _ => "failed");
            return $"output: {value}";
        });
        var left = RaiseBuilders.RunIor<string, string>(string.Concat, r =>
        {
            var value = r.GetOrAccumulate(EitherExtensions.Left<string, string>("nonfatal"), _ => "failed");
            return $"output: {value}";
        });

        Assert.Equal(new Ior<string, string>.Right("output: success"), right);
        Assert.Equal(new Ior<string, string>.Both("nonfatal", "output: failed"), left);
    }

    [Fact]
    public void BindAllIterableNelNesAndMap()
    {
        var iterable = new List<Ior<string, int>>
        {
            new Ior<string, int>.Both("a", 1),
            new Ior<string, int>.Right(2)
        };
        var nel = NonEmptyList<Ior<string, int>>.Of(new Ior<string, int>.Both("a", 1), new Ior<string, int>.Right(2));
        var nes = NonEmptySet<Ior<string, int>>.Of(new Ior<string, int>.Both("a", 1), new Ior<string, int>.Right(2));
        var map = new Dictionary<int, Ior<string, int>>
        {
            [1] = new Ior<string, int>.Both("a", 1),
            [2] = new Ior<string, int>.Right(2)
        };

        var iterableResult = RaiseBuilders.RunIor<string, List<int>>(string.Concat, r => r.BindAllIor(iterable));
        var nelResult = RaiseBuilders.RunIor<string, List<int>>(string.Concat, r => r.BindAllIor(nel));
        var nesResult = RaiseBuilders.RunIor<string, List<int>>(string.Concat, r => r.BindAllIor(nes));
        var mapResult = RaiseBuilders.RunIor<string, Dictionary<int, int>>(string.Concat, r => r.BindAllIor(map));

        Assert.Equal(new Ior<string, List<int>>.Both("a", new List<int> { 1, 2 }), iterableResult, IorListComparer<string, int>.Default);
        Assert.Equal(new Ior<string, List<int>>.Both("a", new List<int> { 1, 2 }), nelResult, IorListComparer<string, int>.Default);
        Assert.IsType<Ior<string, List<int>>.Both>(nesResult);
        Assert.Equal(new Ior<string, Dictionary<int, int>>.Both("a", new Dictionary<int, int> { [1] = 1, [2] = 2 }), mapResult, IorDictionaryComparer<string, int, int>.Default);
    }

    private sealed class IorListComparer<A, B> : IEqualityComparer<Ior<A, List<B>>>
    {
        internal static readonly IorListComparer<A, B> Default = new();

        public bool Equals(Ior<A, List<B>>? x, Ior<A, List<B>>? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null || x.GetType() != y.GetType())
                return false;

            return x switch
            {
                Ior<A, List<B>>.Left left when y is Ior<A, List<B>>.Left right =>
                    EqualityComparer<A>.Default.Equals(left.Value, right.Value),
                Ior<A, List<B>>.Right left when y is Ior<A, List<B>>.Right right =>
                    left.Value.SequenceEqual(right.Value),
                Ior<A, List<B>>.Both left when y is Ior<A, List<B>>.Both right =>
                    EqualityComparer<A>.Default.Equals(left.LeftValue, right.LeftValue) &&
                    left.RightValue.SequenceEqual(right.RightValue),
                _ => false
            };
        }

        public int GetHashCode(Ior<A, List<B>> obj) => obj.GetHashCode();
    }

    private sealed class IorDictionaryComparer<A, K, V> : IEqualityComparer<Ior<A, Dictionary<K, V>>> where K : notnull
    {
        internal static readonly IorDictionaryComparer<A, K, V> Default = new();

        public bool Equals(Ior<A, Dictionary<K, V>>? x, Ior<A, Dictionary<K, V>>? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null || x.GetType() != y.GetType())
                return false;

            return x switch
            {
                Ior<A, Dictionary<K, V>>.Left left when y is Ior<A, Dictionary<K, V>>.Left right =>
                    EqualityComparer<A>.Default.Equals(left.Value, right.Value),
                Ior<A, Dictionary<K, V>>.Right left when y is Ior<A, Dictionary<K, V>>.Right right =>
                    left.Value.Count == right.Value.Count &&
                    left.Value.All(pair => right.Value.TryGetValue(pair.Key, out var value) && EqualityComparer<V>.Default.Equals(pair.Value, value)),
                Ior<A, Dictionary<K, V>>.Both left when y is Ior<A, Dictionary<K, V>>.Both right =>
                    EqualityComparer<A>.Default.Equals(left.LeftValue, right.LeftValue) &&
                    left.RightValue.Count == right.RightValue.Count &&
                    left.RightValue.All(pair => right.RightValue.TryGetValue(pair.Key, out var value) && EqualityComparer<V>.Default.Equals(pair.Value, value)),
                _ => false
            };
        }

        public int GetHashCode(Ior<A, Dictionary<K, V>> obj) => obj.GetHashCode();
    }
}
