using Arrow.Core.Test.Generators;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class GeneratorsTest
{
    [Property]
    public void FunctionAToBShouldReturnSameResultWhenInvokedMultipleTimes(int a)
    {
        foreach (var fn in Gen.Sample(0, 50, ArrowGenerators.FunctionAToB<int, int>(Arb.Default.Int32().Generator)))
        {
            Assert.Equal(fn(a), fn(a));
        }
    }

    [Fact]
    public void FunctionAToBShouldReturnSomeDifferentValues()
    {
        var a = 0;
        var a2 = 1;

        var distinctValues = GivenSamples(ArrowGenerators.FunctionAToB<int, int>(Arb.Default.Int32().Generator))
            .Select(static fn => fn(0))
            .Distinct()
            .Take(2)
            .Count();

        Assert.True(distinctValues > 1);
    }

    [Property]
    public void FunctionABCToDShouldReturnSameResultWhenInvokedMultipleTimes(int a, int b, int c)
    {
        foreach (var fn in Gen.Sample(0, 50, ArrowGenerators.FunctionABCToD<int, int, int, int>(Arb.Default.Int32().Generator)))
        {
            Assert.Equal(fn(a, b, c), fn(a, b, c));
        }
    }

    [Fact]
    public void FunctionABCToDShouldReturnSomeDifferentValues()
    {
        const int a = 0;
        const int a2 = 1;
        const int b = 0;
        const int c = 0;

        var distinctValues = GivenSamples(ArrowGenerators.FunctionABCToD<int, int, int, int>(Arb.Default.Int32().Generator))
            .Select(static fn => fn(0, 0, 0))
            .Distinct()
            .Take(2)
            .Count();

        Assert.True(distinctValues > 1);
    }

    [Fact]
    public void ArbMap2AtLeastOneSampleShouldShareNoKeys()
    {
        var counts = GivenSamples(
                ArrowGenerators.Map2(
                    Arb.Default.Int32().Generator,
                    Arb.Default.Bool().Generator,
                    Arb.Default.Bool().Generator))
            .Select(static maps => maps.Item1.Keys.Intersect(maps.Item2.Keys).Count())
            .ToList();

        Assert.Contains(0, counts);
    }

    [Fact]
    public void ArbMap2AtLeastOneSampleShouldShareSomeKeys()
    {
        var counts = GivenSamples(
                ArrowGenerators.Map2(
                    Arb.Default.Int32().Generator,
                    Arb.Default.Bool().Generator,
                    Arb.Default.Bool().Generator))
            .Select(static maps => maps.Item1.Keys.Intersect(maps.Item2.Keys).Count())
            .ToList();

        Assert.Contains(counts, static count => count > 0);
    }

    [Fact]
    public void ArbMap2NoNullValuesIfArbDoesNotProduceNullables()
    {
        foreach (var sample in GivenSamples(
                     ArrowGenerators.Map2(
                         Arb.Default.Int32().Generator,
                         Arb.Default.Bool().Generator,
                         Arb.Default.Bool().Generator)))
        {
            foreach (var value in sample.Item1.Values)
            {
                _ = AssertionHelpers.ShouldBeInstanceOf<bool>(value);
            }

            foreach (var value in sample.Item2.Values)
            {
                _ = AssertionHelpers.ShouldBeInstanceOf<bool>(value);
            }
        }
    }

    private static IEnumerable<T> GivenSamples<T>(Gen<T> gen, int count = 250) =>
        gen.Sample(0, count);
}
