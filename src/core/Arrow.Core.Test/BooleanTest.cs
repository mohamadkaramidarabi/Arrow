using FsCheck;

namespace Arrow.Core.Test;

public class BooleanTest
{
    [Fact]
    public void MonoidLaws() =>
        LawTesting.TestLaws(new Laws.MonoidLaws<bool>(
            "Boolean",
            true,
            static (x, y) => x && y,
            Arb.From(Gen.Elements(true, false))));
}
