namespace Arrow.Core.Raise;

public delegate A EagerEffect<Error, A>(IRaise<Error> raise);

public delegate Task<A> Effect<Error, A>(IRaise<Error> raise);

public static class RaiseEffect
{
    public static EagerEffect<Error, A> Eager<Error, A>(Func<IRaise<Error>, A> block) =>
        raise => block(raise);

    public static Effect<Error, A> Of<Error, A>(Func<IRaise<Error>, Task<A>> block) =>
        raise => block(raise);

    public static Effect<Error, A> Of<Error, A>(Func<IRaise<Error>, A> block) =>
        raise => Task.FromResult(block(raise));

    public static A Invoke<Error, A>(this EagerEffect<Error, A> effect, IRaise<Error> raise) =>
        effect(raise);

    public static Task<A> InvokeAsync<Error, A>(this Effect<Error, A> effect, IRaise<Error> raise) =>
        effect(raise);

    public static A Bind<Error, A>(this EagerEffect<Error, A> effect, IRaise<Error> raise) =>
        effect.Invoke(raise);

    public static Task<A> BindAsync<Error, A>(this Effect<Error, A> effect, IRaise<Error> raise) =>
        effect.InvokeAsync(raise);
}

public static class RaiseFold
{
    public static B Fold<Error, A, B>(
        this EagerEffect<Error, A> effect,
        Func<Exception, B> catchHandler,
        Func<Error, B> recover,
        Func<A, B> transform) =>
        RaiseExtensions.Fold<Error, A, B>(effect.Invoke, catchHandler, recover, transform);

    public static B Fold<Error, A, B>(
        this EagerEffect<Error, A> effect,
        Func<Error, B> recover,
        Func<A, B> transform) =>
        effect.Fold(static ex => throw ex, recover, transform);

    public static async Task<B> FoldAsync<Error, A, B>(
        this Effect<Error, A> effect,
        Func<Exception, B> catchHandler,
        Func<Error, B> recover,
        Func<A, B> transform)
    {
        var scope = new RaiseScope(isTraced: false);
        var raise = new TypedRaise<Error>(scope);
        try
        {
            var result = await effect.InvokeAsync(raise).ConfigureAwait(false);
            scope.Complete();
            return transform(result);
        }
        catch (RaiseCancellationException ex) when (ReferenceEquals(ex.Scope, scope))
        {
            scope.Complete();
            return recover((Error)ex.Raised!);
        }
        catch (Exception ex)
        {
            scope.Complete();
            return catchHandler(ex.NonFatalOrThrow());
        }
    }

    public static Task<B> FoldAsync<Error, A, B>(
        this Effect<Error, A> effect,
        Func<Error, B> recover,
        Func<A, B> transform) =>
        effect.FoldAsync(static ex => throw ex, recover, transform);
}
