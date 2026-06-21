using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class OptionSpec
{
    [Fact]
    public void EnsureTrueReturnsSome() =>
        Assert.Equal(
            OptionExtensions.Some(5),
            RaiseBuilders.RunOption(r =>
            {
                r.Ensure(true);
                return 5;
            }));

    [Fact]
    public void EnsureFalseReturnsNone() =>
        Assert.Equal(
            OptionExtensions.None<int>(),
            RaiseBuilders.RunOption(r =>
            {
                r.Ensure(false);
                return 5;
            }));

    [Fact]
    public void EnsureNotNullInOptionComputation() =>
        Assert.Equal(
            OptionExtensions.Some(9),
            RaiseBuilders.RunOption(r =>
            {
                r.EnsureNotNull("abc");
                return r.EnsureNotNull("abc").Length * r.EnsureNotNull("abc").Length;
            }));

    [Fact]
    public void EnsureNotNullShortCircuit() =>
        Assert.Equal(
            OptionExtensions.None<int>(),
            RaiseBuilders.RunOption<int>(r =>
            {
                r.EnsureNotNull<string>(null);
                throw new InvalidOperationException("Should not execute");
            }));

    [Fact]
    public void ShortCircuitOption() =>
        Assert.Equal(
            OptionExtensions.None<int>(),
            RaiseBuilders.RunOption<int>(r =>
            {
                r.Bind(OptionExtensions.None<int>());
                throw new InvalidOperationException("Should not execute");
            }));

    [Fact]
    public void SimpleCase() =>
        Assert.Equal(
            OptionExtensions.Some(1),
            RaiseBuilders.RunOption(r => r.Bind(OptionExtensions.Some("s".Length))));

    [Fact]
    public void MultipleTypes() =>
        Assert.Equal(
            OptionExtensions.Some("1"),
            RaiseBuilders.RunOption(r =>
            {
                var number = "s".Length;
                return r.Bind(OptionExtensions.Some(number.ToString()));
            }));

    [Fact]
    public void IfExpression() =>
        Assert.Equal(
            OptionExtensions.Some("1"),
            RaiseBuilders.RunOption(r =>
            {
                var number = r.Bind(OptionExtensions.Some("s".Length));
                return r.Bind(number == 1 ? OptionExtensions.Some(number.ToString()) : OptionExtensions.None<string>());
            }));

    [Fact]
    public void IfExpressionShortCircuit() =>
        Assert.Equal(
            OptionExtensions.None<string>(),
            RaiseBuilders.RunOption<string>(r =>
            {
                var number = r.Bind(OptionExtensions.Some("s".Length));
                return r.Bind(number != 1 ? OptionExtensions.Some(number.ToString()) : OptionExtensions.None<string>());
            }));

    [Fact]
    public void RecoverWorksAsExpected() =>
        Assert.Equal(
            OptionExtensions.Some(3),
            RaiseBuilders.RunOption<int>(r =>
            {
                var one = r.Recover(
                    inner => inner.Bind(OptionExtensions.None<int>()),
                    () => 1);
                var two = r.Bind(OptionExtensions.Some(2));
                return one + two;
            }));
}
