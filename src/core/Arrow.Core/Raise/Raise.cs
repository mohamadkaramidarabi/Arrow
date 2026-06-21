using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Raise;

public interface IRaise<in Error>
{
    
    [DoesNotReturn]
    void Raise(Error error);
}

internal sealed class TypedRaise<Error>(RaiseScope scope) : IRaise<Error>
{
    [DoesNotReturn]
    public void Raise(Error error) => scope.Raise(error);
}

public static class RaiseExtensions
{
    
    public static A Bind<Error, A>(this IRaise<Error> raise, Either<Error, A> either) =>
        either switch
        {
            Either<Error, A>.Left left => throw RaiseError(raise, left.Value),
            Either<Error, A>.Right right => right.Value,
            _ => throw new InvalidOperationException()
        };

    
    public static Dictionary<K, A> BindAll<Error, K, A>(
        this IRaise<Error> raise,
        IReadOnlyDictionary<K, Either<Error, A>> map) where K : notnull =>
        map.ToDictionary(static kv => kv.Key, kv => raise.Bind(kv.Value));

    
    public static List<A> BindAll<Error, A>(this IRaise<Error> raise, IEnumerable<Either<Error, A>> values) =>
        values.Select(raise.Bind).ToList();

    
    public static NonEmptyList<A> BindAll<Error, A>(
        this IRaise<Error> raise,
        NonEmptyList<Either<Error, A>> values) =>
        values.Map(raise.Bind);

    
    public static NonEmptySet<A> BindAll<Error, A>(
        this IRaise<Error> raise,
        NonEmptySet<Either<Error, A>> values) =>
        NonEmptySet<A>.FromSet(values.Map(raise.Bind).All.ToHashSet());

    
    public static void Ensure<Error>(this IRaise<Error> raise, bool condition, Func<Error> error)
    {
        if (!condition)
            raise.Raise(error());
    }

    
    public static B EnsureNotNull<Error, B>(this IRaise<Error> raise, B? value, Func<Error> error) =>
        value ?? throw RaiseError(raise, error());

    
    public static A WithError<Error, OtherError, A>(
        this IRaise<Error> raise,
        Func<OtherError, Error> transform,
        Func<IRaise<OtherError>, A> block) =>
        block(new TransformedRaise<Error, OtherError>(raise, transform));

    internal sealed class TransformedRaise<Error, OtherError>(
        IRaise<Error> outer,
        Func<OtherError, Error> transform) : IRaise<OtherError>
    {
        [DoesNotReturn]
        public void Raise(OtherError error) => outer.Raise(transform(error));
    }

    
    public static A Recover<Error, A>(Func<IRaise<Error>, A> block, Func<Error, A> recover) =>
        FoldOrThrow(block, recover, static a => a);

    
    public static A Recover<Error, A>(
        Func<IRaise<Error>, A> block,
        Func<Error, A> recover,
        Func<Exception, A> catchHandler) =>
        Fold(block, catchHandler, recover, static a => a);

    
    public static A Recover<TException, Error, A>(
        Func<IRaise<Error>, A> block,
        Func<Error, A> recover,
        Func<TException, A> catchHandler) where TException : Exception =>
        Fold(block, t => t is TException ex ? catchHandler(ex) : throw t, recover, static a => a);

    
    public static A Catch<A>(Func<A> block, Func<Exception, A> catchHandler)
    {
        try
        {
            return block();
        }
        catch (Exception ex)
        {
            return catchHandler(ex.NonFatalOrThrow());
        }
    }

    
    public static B Catch<A, B>(Func<A> block, Func<A, B> transform, Func<Exception, B> catchHandler)
    {
        try
        {
            return transform(block());
        }
        catch (Exception ex)
        {
            return catchHandler(ex.NonFatalOrThrow());
        }
    }

    
    public static A Catch<TException, A>(Func<A> block, Func<TException, A> catchHandler)
        where TException : Exception =>
        Catch(block, t => t is TException ex ? catchHandler(ex) : throw t);

    
    public static A Merge<A>(Func<IRaise<A>, A> block) =>
        Recover(block, static a => a);

    public static B FoldOrThrow<Error, A, B>(
        Func<IRaise<Error>, A> block,
        Func<Error, B> recover,
        Func<A, B> transform) =>
        Fold(block, static ex => throw ex, recover, transform);

    public static B Fold<Error, A, B>(
        Func<IRaise<Error>, A> block,
        Func<Exception, B> catchHandler,
        Func<Error, B> recover,
        Func<A, B> transform)
    {
        var scope = new RaiseScope(isTraced: false);
        var raise = new TypedRaise<Error>(scope);
        try
        {
            var result = block(raise);
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

    [DoesNotReturn]
    private static Exception RaiseError<Error>(IRaise<Error> raise, Error error)
    {
        raise.Raise(error);
        throw new InvalidOperationException("Unreachable");
    }
}
