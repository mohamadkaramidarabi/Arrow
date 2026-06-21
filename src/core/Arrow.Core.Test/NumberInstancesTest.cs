using Arrow.Core.Test.Laws;
using FsCheck;

namespace Arrow.Core.Test;

public class NumberInstancesTest
{
    [Fact]
    public void ByteSemiring() =>
        LawTesting.TestLaws(
            new SemiringLaws<byte>(
                "Byte",
                0,
                static (x, y) => (byte)(x + y),
                1,
                static (x, y) => (byte)(x * y),
                Arb.Default.Byte()));

    [Fact]
    public void ShortSemiring() =>
        LawTesting.TestLaws(
            new SemiringLaws<short>(
                "Short",
                0,
                static (x, y) => (short)(x + y),
                1,
                static (x, y) => (short)(x * y),
                Arb.Default.Int16()));

    [Fact]
    public void IntSemiring() =>
        LawTesting.TestLaws(
            new SemiringLaws<int>(
                "Int",
                0,
                static (x, y) => x + y,
                1,
                static (x, y) => x * y,
                Arb.Default.Int32()));

    [Fact]
    public void LongSemiring() =>
        LawTesting.TestLaws(
            new SemiringLaws<long>(
                "Long",
                0,
                static (x, y) => x + y,
                1,
                static (x, y) => x * y,
                Arb.Default.Int64()));

    [Fact]
    public void ByteMonoid() =>
        LawTesting.TestLaws(
            new MonoidLaws<byte>(
                "Byte",
                0,
                static (x, y) => (byte)(x + y),
                Arb.Default.Byte()));

    [Fact]
    public void ShortMonoid() =>
        LawTesting.TestLaws(
            new MonoidLaws<short>(
                "Short",
                0,
                static (x, y) => (short)(x + y),
                Arb.Default.Int16()));

    [Fact]
    public void IntMonoid() =>
        LawTesting.TestLaws(
            new MonoidLaws<int>(
                "Int",
                0,
                static (x, y) => x + y,
                Arb.Default.Int32()));

    [Fact]
    public void LongMonoid() =>
        LawTesting.TestLaws(
            new MonoidLaws<long>(
                "Long",
                0,
                static (x, y) => x + y,
                Arb.Default.Int64()));
}
