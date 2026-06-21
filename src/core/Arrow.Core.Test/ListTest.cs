using FsCheck;
using FsCheck.Xunit;
using Arrow.Core.Test.Laws;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class ListTest
{
    [Fact]
    public void MonoidLaws() =>
        LawTesting.TestLaws(
            new MonoidLaws<List<int>>(
                "List",
                [],
                static (x, y) => [..x, ..y],
                Arb.Default.List<Int32>(),
                static (left, right) => left.SequenceEqual(right)));

    [Property]
    public void MapNotNullOk(List<int> list)
    {
        var actual = list.Select(static n => n % 2 == 0 ? n.ToString() : null)
            .Where(static s => s is not null)
            .Select(static s => s!)
            .ToList();
        var expected = list.Where(static n => n % 2 == 0).Select(static n => n.ToString()).ToList();
        Assert.Equal(expected, actual);
    }
}
