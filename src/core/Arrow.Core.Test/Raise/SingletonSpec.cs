using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class SingletonSpec
{
    [Fact]
    public void ExceptionProperlyEscapesSingleton() =>
        Assert.Throws<ArithmeticException>(() =>
            RaiseBuilders.Singleton(() => 0, _ => throw new ArithmeticException()));

    [Fact]
    public void SuccessfulPath() =>
        Assert.Equal(42, RaiseBuilders.Singleton(() => 0, _ => 42));

    [Fact]
    public void AnyRaiseInfoIsProperlySwallowed()
    {
        Assert.Equal("recovered", RaiseBuilders.Singleton(() => "recovered", r =>
        {
            r.Raise();
            return "never";
        }));
        Assert.Equal(1, RaiseBuilders.Singleton(() => 1, r =>
        {
            r.Raise();
            return 0;
        }));
    }

    [Fact]
    public void Ensure() =>
        Assert.Equal("ok", RaiseBuilders.Singleton(() => "recovered", r =>
        {
            r.Ensure(true);
            return "ok";
        }));

    [Fact]
    public void EnsureFalseRecovers() =>
        Assert.Equal("recovered", RaiseBuilders.Singleton(() => "recovered", r =>
        {
            r.Ensure(false);
            return "never";
        }));

    [Fact]
    public void OptionBind() =>
        Assert.Equal(10, RaiseBuilders.Singleton(() => 1, r => r.Bind(OptionExtensions.Some(10))));

    [Fact]
    public void BindNullable() =>
        Assert.Equal(10, RaiseBuilders.Singleton(() => 1, r => r.BindNullable("10")!.Length * 5));

    [Fact]
    public void EnsureNotNull() =>
        Assert.Equal(3, RaiseBuilders.Singleton(() => 1, r => r.EnsureNotNull("abc").Length));

    [Fact]
    public void MapNullableBindAll()
    {
        var map = new Dictionary<int, string?> { [1] = "a", [2] = "bb" };
        var result = RaiseBuilders.Singleton(() => new Dictionary<int, string>(), r => r.BindAllNullable(map));
        Assert.Equal(2, result.Count);
        Assert.Equal("bb", result[2]);
    }

    [Fact]
    public void IterableOptionBindAll()
    {
        var values = new List<Option<int>> { OptionExtensions.Some(1), OptionExtensions.Some(2) };
        var result = RaiseBuilders.Singleton(() => new List<int>(), r => r.BindAllOption(values));
        Assert.Equal(new List<int> { 1, 2 }, result);
    }

    [Fact]
    public void NelAndNesBindAll()
    {
        var nel = NonEmptyList<Option<int>>.Of(OptionExtensions.Some(1), OptionExtensions.Some(2));
        var nes = NonEmptySet<Option<int>>.Of(OptionExtensions.Some(1), OptionExtensions.Some(2));

        var nelResult = RaiseBuilders.Singleton(() => (NonEmptyList<int>?)null, r => r.BindAllOption(nel));
        var nesResult = RaiseBuilders.Singleton(() => (NonEmptySet<int>?)null, r => r.BindAllOption(nes));

        Assert.NotNull(nelResult);
        Assert.Equal(2, nelResult.Value.Count);
        Assert.NotNull(nesResult);
        Assert.Equal(2, nesResult.Value.Count);
    }

    [Fact]
    public void Recover()
    {
        var result = RaiseBuilders.Singleton(() => "", r =>
            r.Recover(
                inner => inner.BindNullable<string>(null)!.ToString(),
                () => "recovered"));

        Assert.Equal("recovered", result);
    }
}
