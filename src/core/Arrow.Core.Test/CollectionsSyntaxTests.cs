namespace Arrow.Core.Test;

public class CollectionsSyntaxTests
{
    [Fact]
    public void Tail() =>
        Assert.Equal([2, 3], IterableExtensions.Tail(new[] { 1, 2, 3 }));

    [Fact]
    public void PrependTo() =>
        Assert.Equal([1, 2, 3], 1.PrependTo([2, 3]));

    [Fact]
    public void Destructured()
    {
        var source = new[] { 1, 2, 3 };
        var head = source.First();
        var tail = IterableExtensions.Tail(source);

        Assert.Equal(1, head);
        Assert.Equal([2, 3], tail);
    }
}
