using FsCheck;

namespace Arrow.Core.Test.Laws;

public sealed class SemigroupLaws<F>(string name, Func<F, F, F> combine, Arbitrary<F> gen, Func<F, F, bool>? eq = null) : ILawSet
{
    public IReadOnlyList<Law> Laws { get; } =
    [
        new Law($"Semigroup Laws ({name}): associativity", () =>
            Prop.ForAll(gen, gen, gen, (a, b, c) =>
                (eq ?? EqualityComparer<F>.Default.Equals)(combine(combine(a, b), c), combine(a, combine(b, c)))).QuickCheckThrowOnFailure())
    ];
}

public sealed class MonoidLaws<F>(
    string name,
    F empty,
    Func<F, F, F> combine,
    Arbitrary<F> gen,
    Func<F, F, bool>? eq = null) : ILawSet
{
    public IReadOnlyList<Law> Laws { get; } =
        new SemigroupLaws<F>(name, combine, gen, eq).Laws
            .Concat([
                new Law($"Monoid Laws ({name}): Left identity", () =>
                    Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combine(empty, a), a)).QuickCheckThrowOnFailure()),
                new Law($"Monoid Laws ({name}): Right identity", () =>
                    Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combine(a, empty), a)).QuickCheckThrowOnFailure()),
                new Law($"Monoid Laws ({name}): combineAll should be derived", () =>
                    Prop.ForAll(Arb.From(Gen.ListOf(gen.Generator).Select(static l => l.ToList())), list =>
                        (eq ?? EqualityComparer<F>.Default.Equals)(
                            list.Aggregate(empty, combine),
                            list.Count == 0 ? empty : list.Aggregate(combine))).QuickCheckThrowOnFailure()),
                new Law($"Monoid Laws ({name}): combineAll of empty list is empty", () =>
                    Prop.ForAll(Arb.From(Gen.Constant(Unit.Value)), _ =>
                        (eq ?? EqualityComparer<F>.Default.Equals)(Enumerable.Empty<F>().Aggregate(empty, combine), empty)).QuickCheckThrowOnFailure())
            ]).ToList();
}

public sealed class SemiringLaws<F>(
    string name,
    F zero,
    Func<F, F, F> combine,
    F one,
    Func<F, F, F> combineMultiplicate,
    Arbitrary<F> gen,
    Func<F, F, bool>? eq = null) : ILawSet
{
    public IReadOnlyList<Law> Laws { get; } =
    [
        new Law($"Semiring Laws ({name}): Additive commutativity", () =>
            Prop.ForAll(gen, gen, (a, b) => (eq ?? EqualityComparer<F>.Default.Equals)(combine(a, b), combine(b, a))).QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Additive identity", () =>
            Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combine(a, zero), a)).QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Additive associativity", () =>
            Prop.ForAll(gen, gen, gen, (a, b, c) => (eq ?? EqualityComparer<F>.Default.Equals)(combine(combine(a, b), c), combine(a, combine(b, c))))
            .QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Multiplicative commutativity", () =>
            Prop.ForAll(gen, gen, (a, b) => (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(a, b), combineMultiplicate(b, a)))
            .QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Multiplicative identity", () =>
            Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(a, one), a)).QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Multiplicative associativity", () =>
            Prop.ForAll(gen, gen, gen, (a, b, c) =>
                (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(combineMultiplicate(a, b), c), combineMultiplicate(a, combineMultiplicate(b, c))))
            .QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Left distributivity", () =>
            Prop.ForAll(gen, gen, gen, (a, b, c) =>
                (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(a, combine(b, c)), combine(combineMultiplicate(a, b), combineMultiplicate(a, c))))
            .QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Right distributivity", () =>
            Prop.ForAll(gen, gen, gen, (a, b, c) =>
                (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(combine(a, b), c), combine(combineMultiplicate(a, c), combineMultiplicate(b, c))))
            .QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Multiplicative absorption left", () =>
            Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(zero, a), zero)).QuickCheckThrowOnFailure()),
        new Law($"Semiring Laws ({name}): Multiplicative absorption right", () =>
            Prop.ForAll(gen, a => (eq ?? EqualityComparer<F>.Default.Equals)(combineMultiplicate(a, zero), zero)).QuickCheckThrowOnFailure())
    ];
}
