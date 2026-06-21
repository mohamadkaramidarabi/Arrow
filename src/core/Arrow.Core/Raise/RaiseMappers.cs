namespace Arrow.Core.Raise;

using System.Reflection;

public static class RaiseMappers
{
    public static Either<Error, A> ToEither<Error, A>(this EagerEffect<Error, A> effect) =>
        RaiseBuilders.RunEither<Error, A>(effect.Invoke);

    public static async Task<Either<Error, A>> ToEitherAsync<Error, A>(this Effect<Error, A> effect) =>
        await RaiseFold.FoldAsync<Error, A, Either<Error, A>>(
            effect,
            static ex => throw ex,
            static e => new Either<Error, A>.Left(e),
            static a => new Either<Error, A>.Right(a)).ConfigureAwait(false);

    public static Ior<Error, A> ToIor<Error, A>(this EagerEffect<Error, A> effect) =>
        RaiseFold.Fold<Error, A, Ior<Error, A>>(
            effect,
            static ex => throw ex,
            static e => (Ior<Error, A>)new Ior<Error, A>.Left(e),
            static a => new Ior<Error, A>.Right(a));

    public static async Task<Ior<Error, A>> ToIorAsync<Error, A>(this Effect<Error, A> effect) =>
        await RaiseFold.FoldAsync<Error, A, Ior<Error, A>>(
            effect,
            static ex => throw ex,
            static e => (Ior<Error, A>)new Ior<Error, A>.Left(e),
            static a => new Ior<Error, A>.Right(a)).ConfigureAwait(false);

    public static A? GetOrNull<Error, A>(this EagerEffect<Error, A> effect) =>
        typeof(A).IsValueType
            ? GetOrNullStruct<Error, A>(effect)
            : GetOrNullReference<Error, A>(effect);

    public static Task<A?> GetOrNullAsync<Error, A>(this Effect<Error, A> effect) =>
        typeof(A).IsValueType
            ? GetOrNullAsyncStruct<Error, A>(effect)
            : GetOrNullAsyncReference<Error, A>(effect);

    private static A? GetOrNullStruct<Error, A>(EagerEffect<Error, A> effect) =>
        (A?)GetOrNullStructMethod.MakeGenericMethod(typeof(Error), typeof(A)).Invoke(null, [effect])!;

    private static A? GetOrNullReference<Error, A>(EagerEffect<Error, A> effect) =>
        (A?)GetOrNullReferenceMethod.MakeGenericMethod(typeof(Error), typeof(A)).Invoke(null, [effect])!;

    private static Task<A?> GetOrNullAsyncStruct<Error, A>(Effect<Error, A> effect) =>
        (Task<A?>)GetOrNullAsyncStructMethod.MakeGenericMethod(typeof(Error), typeof(A)).Invoke(null, [effect])!;

    private static Task<A?> GetOrNullAsyncReference<Error, A>(Effect<Error, A> effect) =>
        (Task<A?>)GetOrNullAsyncReferenceMethod.MakeGenericMethod(typeof(Error), typeof(A)).Invoke(null, [effect])!;

    private static readonly MethodInfo GetOrNullStructMethod =
        typeof(RaiseMappers).GetMethod(nameof(GetOrNullStructImpl), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly MethodInfo GetOrNullReferenceMethod =
        typeof(RaiseMappers).GetMethod(nameof(GetOrNullReferenceImpl), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly MethodInfo GetOrNullAsyncStructMethod =
        typeof(RaiseMappers).GetMethod(nameof(GetOrNullAsyncStructImpl), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly MethodInfo GetOrNullAsyncReferenceMethod =
        typeof(RaiseMappers).GetMethod(nameof(GetOrNullAsyncReferenceImpl), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static A? GetOrNullStructImpl<Error, A>(EagerEffect<Error, A> effect) where A : struct =>
        RaiseExtensions.FoldOrThrow<Error, A, A?>(
            effect.Invoke,
            static _ => NullableStructRaise.Null<A>(),
            static a => a);

    private static A? GetOrNullReferenceImpl<Error, A>(EagerEffect<Error, A> effect) where A : class =>
        RaiseExtensions.FoldOrThrow<Error, A, A?>(
            effect.Invoke,
            static _ => NullableReferenceRaise.Null<A>(),
            static a => a);

    private static Task<A?> GetOrNullAsyncStructImpl<Error, A>(Effect<Error, A> effect) where A : struct =>
        effect.FoldAsync(static _ => NullableStructRaise.Null<A>(), static a => a);

    private static Task<A?> GetOrNullAsyncReferenceImpl<Error, A>(Effect<Error, A> effect) where A : class =>
        effect.FoldAsync(static _ => NullableReferenceRaise.Null<A>(), static a => a);

    public static Option<A> ToOption<Error, A>(
        this EagerEffect<Error, A> effect,
        Func<Error, Option<A>> recover) =>
        effect.Fold(recover, static a => OptionExtensions.Some(a));

    public static OperationResult<A> ToResult<Error, A>(
        this EagerEffect<Error, A> effect,
        Func<Error, OperationResult<A>> recover) =>
        effect.Fold(
            static ex => OperationResult<A>.Failure(ex),
            recover,
            static a => OperationResult<A>.Success(a));

    public static OperationResult<A> ToResult<A>(this EagerEffect<Exception, A> effect) =>
        RaiseBuilders.RunResult(effect.Invoke);

    public static A GetOrElse<Error, A>(this EagerEffect<Error, A> effect, Func<Error, A> recover) =>
        RaiseExtensions.Recover(effect.Invoke, recover);

    public static async Task<A> GetOrElseAsync<Error, A>(
        this Effect<Error, A> effect,
        Func<Error, A> recover) =>
        await effect.FoldAsync(recover, static a => a).ConfigureAwait(false);
}

public static class RaiseErrorHandlers
{
    public static Effect<OtherError, A> Recover<Error, OtherError, A>(
        this Effect<Error, A> effect,
        Func<IRaise<OtherError>, Error, A> recover) =>
        RaiseEffect.Of<OtherError, A>(async raise =>
            await RaiseAsync.RecoverAsync<Error, A>(
                r => effect.InvokeAsync(r),
                e => recover(raise, e)).ConfigureAwait(false));

    public static Effect<Error, A> Catch<Error, A>(
        this Effect<Error, A> effect,
        Func<IRaise<Error>, Exception, A> catchHandler) =>
        RaiseEffect.Of<Error, A>(async raise =>
            await RaiseAsync.CatchAsync(
                () => effect.InvokeAsync(raise),
                ex => catchHandler(raise, ex)).ConfigureAwait(false));

    public static Effect<OtherError, A> MapError<Error, OtherError, A>(
        this Effect<Error, A> effect,
        Func<Error, OtherError> transform) =>
        RaiseEffect.Of<OtherError, A>(raise =>
            raise.WithError(transform, r => effect.Invoke(r)));

    public static EagerEffect<OtherError, A> Recover<Error, OtherError, A>(
        this EagerEffect<Error, A> effect,
        Func<IRaise<OtherError>, Error, A> recover) =>
        RaiseEffect.Eager<OtherError, A>(raise =>
            RaiseExtensions.Recover<Error, A>(
                r => effect.Invoke(r),
                e => recover(raise, e)));

    public static EagerEffect<Error, A> Catch<Error, A>(
        this EagerEffect<Error, A> effect,
        Func<IRaise<Error>, Exception, A> catchHandler) =>
        RaiseEffect.Eager<Error, A>(raise =>
            RaiseExtensions.Catch(
                () => effect.Invoke(raise),
                ex => catchHandler(raise, ex)));

    public static EagerEffect<OtherError, A> MapError<Error, OtherError, A>(
        this EagerEffect<Error, A> effect,
        Func<Error, OtherError> transform) =>
        RaiseEffect.Eager<OtherError, A>(raise =>
            raise.WithError(transform, r => effect.Invoke(r)));
}

internal static class RaiseAsync
{
    public static async Task<A> RecoverAsync<Error, A>(
        Func<IRaise<Error>, Task<A>> block,
        Func<Error, A> recover)
    {
        var scope = new RaiseScope(isTraced: false);
        var raise = new TypedRaise<Error>(scope);
        try
        {
            var result = await block(raise).ConfigureAwait(false);
            scope.Complete();
            return result;
        }
        catch (RaiseCancellationException ex) when (ReferenceEquals(ex.Scope, scope))
        {
            scope.Complete();
            return recover((Error)ex.Raised!);
        }
    }

    public static async Task<A> CatchAsync<A>(Func<Task<A>> block, Func<Exception, A> catchHandler)
    {
        try
        {
            return await block().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return catchHandler(ex.NonFatalOrThrow());
        }
    }
}
