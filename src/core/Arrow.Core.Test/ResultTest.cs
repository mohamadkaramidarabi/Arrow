using FsCheck.Xunit;

namespace Arrow.Core.Test;

public class ResultTest
{
    [Property(MaxTest = 20)]
    public void FlatMap(int a, int b)
    {
        a = int.Clamp(a, 0, 3);
        b = int.Clamp(b, 0, 1);

        static int Block(int i) => 100 / i;
        static string Transform(int i, int j) => (i / j).ToString();

        OperationResult<string> Expected()
        {
            try
            {
                var value = Block(a);
                try
                {
                    return OperationResult<string>.Success(Transform(value, b));
                }
                catch (Exception ex)
                {
                    return OperationResult<string>.Failure(ex);
                }
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Failure(ex);
            }
        }

        var expected = Expected();
        var actual = OperationResultExtensions.FlatMap(
            Try(() => Block(a)),
            value => Try(() => Transform(value, b)));

        if (actual.IsSuccess)
            Assert.Equal(expected.Value, actual.Value);
        else
            Assert.Equal(expected.Exception?.GetType(), actual.Exception?.GetType());
    }

    private static OperationResult<T> Try<T>(Func<T> block)
    {
        try
        {
            return OperationResult<T>.Success(block());
        }
        catch (Exception ex)
        {
            return OperationResult<T>.Failure(ex);
        }
    }
}
