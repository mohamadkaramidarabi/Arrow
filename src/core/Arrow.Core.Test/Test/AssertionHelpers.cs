namespace Arrow.Core.Test;

public static class AssertionHelpers
{
    public static T ShouldBeTypeOf<T>(object? value) where T : class
    {
        Assert.IsType<T>(value);
        return (T)value!;
    }

    public static T ShouldBeInstanceOf<T>(object? value)
    {
        Assert.IsAssignableFrom<T>(value);
        return (T)value!;
    }

    public static T ShouldThrow<T>(Action block) where T : Exception =>
        Assert.Throws<T>(block);
}
