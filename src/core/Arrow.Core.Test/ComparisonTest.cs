using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class ComparisonTest
{
    [Property]
    public void GenericSort2(int ageA, string nameA, int ageB, string nameB)
    {
        var a = new Person(Math.Abs(ageA % 101), nameA ?? string.Empty);
        var b = new Person(Math.Abs(ageB % 101), nameB ?? string.Empty);
        var (first, second) = Comparison.Sort(a, b);
        var expected = new List<Person> { a, b };
        expected.Sort(Person.Comparator);

        Assert.Equal(expected[0], first);
        Assert.Equal(expected[1], second);
    }

    [Property]
    public void GenericSort3(int ageA, string nameA, int ageB, string nameB, int ageC, string nameC)
    {
        var a = new Person(Math.Abs(ageA % 101), nameA ?? string.Empty);
        var b = new Person(Math.Abs(ageB % 101), nameB ?? string.Empty);
        var c = new Person(Math.Abs(ageC % 101), nameC ?? string.Empty);
        var (first, second, third) = Comparison.Sort(a, b, c);
        var expected = new List<Person> { a, b, c };
        expected.Sort(Person.Comparator);

        Assert.Equal(expected[0], first);
        Assert.Equal(expected[1], second);
        Assert.Equal(expected[2], third);
    }

    [Property]
    public void GenericSortAll(int ageA, string nameA, int[] ages, string[] names)
    {
        var a = new Person(Math.Abs(ageA % 101), nameA ?? string.Empty);
        var rest = ages.Zip(names, static (age, name) => new Person(Math.Abs(age % 101), name ?? string.Empty)).ToArray();
        var actual = Comparison.Sort(a, rest);
        var expected = new List<Person> { a };
        expected.AddRange(rest);
        expected.Sort(Person.Comparator);

        Assert.Equal(expected, actual);
    }

    [Property]
    public void ByteSort2(byte a, byte b)
    {
        var (first, second) = Comparison.Sort(a, b);
        var expected = new List<byte> { a, b };
        expected.Sort();
        Assert.Equal(expected[0], first);
        Assert.Equal(expected[1], second);
    }

    [Property]
    public void ShortSort3(short a, short b, short c)
    {
        var (first, second, third) = Comparison.Sort(a, b, c);
        var expected = new List<short> { a, b, c };
        expected.Sort();
        Assert.Equal(expected[0], first);
        Assert.Equal(expected[1], second);
        Assert.Equal(expected[2], third);
    }

    [Property]
    public void IntSortAll(int a, int b, int c, int d)
    {
        var actual = Comparison.Sort(a, b, c, d);
        var expected = new List<int> { a, b, c, d };
        expected.Sort();
        Assert.Equal(expected, actual);
    }

    [Property]
    public void LongSortAll(long a, long b, long c, long d)
    {
        var actual = Comparison.Sort(a, b, c, d);
        var expected = new List<long> { a, b, c, d };
        expected.Sort();
        Assert.Equal(expected, actual);
    }

    [Property]
    public void SortVarargComparator(int ageA, string nameA, int[] ages, string[] names)
    {
        var a = new Person(Math.Abs(ageA % 101), nameA ?? string.Empty);
        var rest = ages.Zip(names, static (age, name) => new Person(Math.Abs(age % 101), name ?? string.Empty)).ToArray();
        var actual = Comparison.Sort(a, Person.Comparator, rest);
        var expected = new List<Person> { a };
        expected.AddRange(rest);
        expected.Sort(Person.Comparator);

        Assert.Equal(expected, actual);
    }

    public sealed class Person(int age, string name) : IComparable<Person>
    {
        public int Age { get; } = age;
        public string Name { get; } = name;

        public static IComparer<Person> Comparator { get; } =
            Comparer<Person>.Create(static (left, right) =>
            {
                var byAge = left.Age.CompareTo(right.Age);
                return byAge != 0 ? byAge : string.CompareOrdinal(left.Name, right.Name);
            });

        public int CompareTo(Person? other) =>
            other is null ? 1 : Comparator.Compare(this, other);

        public override bool Equals(object? obj) =>
            obj is Person other && Age == other.Age && Name == other.Name;

        public override int GetHashCode() => HashCode.Combine(Age, Name);

        public override string ToString() => $"Person(age={Age}, name='{Name}')";
    }

}
