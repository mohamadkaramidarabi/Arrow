using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class ResultSpec
{
    private static readonly Exception Boom = new InvalidOperationException("Boom!");

    [Fact]
    public void ResultException() =>
        Assert.True(RaiseBuilders.RunResult<int>(_ => throw Boom).IsFailure);

    [Fact]
    public void ResultSuccess()
    {
        var result = RaiseBuilders.RunResult(_ => 1);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void ResultRaise() =>
        Assert.True(RaiseBuilders.RunResult<int>(r =>
        {
            r.Raise(Boom);
            return 0;
        }).IsFailure);

    [Fact]
    public void RecoverWorksAsExpected()
    {
        var result = RaiseBuilders.RunResult(r =>
        {
            var one = r.Recover(
                inner => inner.Bind(OperationResult<int>.Failure(Boom)),
                _ => 1);
            var two = r.Bind(OperationResult<int>.Success(2));
            return one + two;
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void BindReturnsInputResult()
    {
        var success = OperationResult<int>.Success(10);
        var failure = OperationResult<int>.Failure(new ArgumentException("bad"));

        var boundSuccess = RaiseBuilders.RunResult(r => r.Bind(success));
        var boundFailure = RaiseBuilders.RunResult(r => r.Bind(failure));

        Assert.True(boundSuccess.IsSuccess);
        Assert.Equal(10, boundSuccess.Value);
        Assert.True(boundFailure.IsFailure);
    }

    [Fact]
    public void BindAllMap()
    {
        var allGood = new Dictionary<int, OperationResult<int>>
        {
            [1] = OperationResult<int>.Success(10),
            [2] = OperationResult<int>.Success(20)
        };
        var oneBad = new Dictionary<int, OperationResult<int>>
        {
            [1] = OperationResult<int>.Success(10),
            [2] = OperationResult<int>.Failure(new InvalidOperationException("fail"))
        };

        var ok = RaiseBuilders.RunResult(r => r.BindAllResult(allGood));
        var failed = RaiseBuilders.RunResult(r => r.BindAllResult(oneBad));

        Assert.True(ok.IsSuccess);
        Assert.Equal(2, ok.Value.Count);
        Assert.True(failed.IsFailure);
    }

    [Fact]
    public void BindAllIterableAndNelAndNes()
    {
        var list = new[]
        {
            OperationResult<int>.Success(1),
            OperationResult<int>.Success(2)
        };
        var nel = NonEmptyList<OperationResult<int>>.Of(OperationResult<int>.Success(1), OperationResult<int>.Success(2));
        var nes = NonEmptySet<OperationResult<int>>.Of(OperationResult<int>.Success(1), OperationResult<int>.Success(2));

        var listResult = RaiseBuilders.RunResult(r => r.BindAllResult(list));
        var nelResult = RaiseBuilders.RunResult(r => r.BindAllResult(nel));
        var nesResult = RaiseBuilders.RunResult(r => r.BindAllResult(nes));

        Assert.True(listResult.IsSuccess);
        Assert.Equal(new List<int> { 1, 2 }, listResult.Value);
        Assert.True(nelResult.IsSuccess);
        Assert.Equal(2, nelResult.Value.Count);
        Assert.True(nesResult.IsSuccess);
        Assert.Equal(2, nesResult.Value.Count);
    }

    [Fact]
    public void RecoverMapsFailureToFallback()
    {
        var bad = OperationResult<int>.Failure(Boom);
        var result = RaiseBuilders.RunResult(r =>
            r.Recover(inner => inner.Bind(bad).ToString(), _ => "recovered"));

        Assert.True(result.IsSuccess);
        Assert.Equal("recovered", result.Value);
    }
}
