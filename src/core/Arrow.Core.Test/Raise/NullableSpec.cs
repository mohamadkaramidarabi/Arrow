using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class NullableSpec
{
    [Fact]
    public void EnsureTrueInNullableComputation() =>
        Assert.Equal(
            7,
            RaiseBuilders.RunNullable(r =>
            {
                r.Ensure(true);
                return 7;
            }));

    [Fact]
    public void EnsureNotNullInNullableComputation() =>
        Assert.Equal(
            9,
            RaiseBuilders.RunNullable(r =>
            {
                var value = r.EnsureNotNull("abc");
                return value.Length * value.Length;
            }));

    [Fact]
    public void SimpleCase() =>
        Assert.Equal(
            1,
            RaiseBuilders.RunNullable(r => r.BindNullable("s".Length.ToString())!.Length));

    [Fact]
    public void MultipleTypes() =>
        Assert.Equal(
            "1",
            RaiseBuilders.RunNullable(r =>
            {
                var number = "s".Length;
                return r.BindNullable(number.ToString());
            }));

    [Fact]
    public void BindingOptionInNullable() =>
        Assert.Equal(
            "1",
            RaiseBuilders.RunNullable(r =>
            {
                var number = OptionExtensions.Some("s".Length);
                return r.Bind(number.Map(i => i.ToString()));
            }));

    [Fact]
    public void IfExpression() =>
        Assert.Equal(
            "1",
            RaiseBuilders.RunNullable(r =>
            {
                var number = r.BindNullable("s".Length.ToString())!.Length;
                return r.BindNullable(number == 1 ? number.ToString() : null);
            }));

    [Fact]
    public void IfExpressionShortCircuit() =>
        Assert.Null(
            RaiseBuilders.RunNullable<string>(r =>
            {
                var number = r.BindNullable("s".Length.ToString())!.Length;
                return r.BindNullable(number != 1 ? number.ToString() : null);
            }));

    [Fact]
    public void RecoverWorksAsExpected() =>
        Assert.Equal(
            3,
            RaiseBuilders.RunNullable(r =>
            {
                var one = r.Recover(
                    inner =>
                    {
                        inner.BindNullable<string>(null);
                        return 0;
                    },
                    () => 1);
                var two = r.Bind(OptionExtensions.Some(2));
                return one + two;
            }));
}
