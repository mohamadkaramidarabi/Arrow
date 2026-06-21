# Arrow (.NET)

A .NET port of [Arrow](https://github.com/arrow-kt/arrow), starting from the Kotlin `arrow-core` library. Arrow.Core provides functional programming abstractions for C#:

- **Sum types:** `Option<A>`, `Either<A,B>`, `Ior<A,B>`
- **Non-empty collections:** `NonEmptyList<E>`, `NonEmptySet<E>`
- **Raise DSL:** typed, short-circuiting error handling (`RunEither`, `RunOption`, `RunNullable`, `RunResult`, `RunIor`)
- **Effects:** reusable `EagerEffect<Error,A>` and async `Effect<Error,A>`
- **Collection utilities:** zip, align, padZip, mapOrAccumulate on iterables, sequences, and dictionaries
- **Memoization, tuples, comparison helpers**

**Requirements:** .NET 10, C# preview, nullable reference types enabled.

## Quick example

```csharp
using Arrow.Core;
using Arrow.Core.Raise;

Either<string, int> result = RaiseBuilders.RunEither<string, int>(raise =>
{
    raise.Ensure(!string.IsNullOrWhiteSpace(input), () => "empty input");
    var value = raise.Bind(ParseInt(input));   // Either<string, int>
    return value * 2;
});

string message = result.Fold(
    err => $"Error: {err}",
    n   => $"Result: {n}");
```

## Documentation

**[Full library guide with examples →](docs/GUIDE.md)**

The guide covers:

- Option, Either, Ior, and OperationResult
- All Raise builders and Effect combinators
- Error accumulation (parallel validation)
- Non-empty collections and iterable/map extensions
- A complete user-registration walkthrough

## Build and test

```bash
dotnet build Arrow.sln
dotnet test Arrow.sln
```

## Publishing

NuGet releases use **trusted publishing** (OIDC, no stored API keys). See **[docs/PUBLISHING.md](docs/PUBLISHING.md)** for setup and release steps.

```bash
git tag v0.1.0
git push origin v0.1.0
```

## Project layout

```
Arrow.sln
src/core/
  Arrow.Core/          # Library (net10.0, no NuGet dependencies)
  Arrow.Core.Test/     # xUnit + FsCheck tests
docs/
  GUIDE.md             # Full documentation
```

## License

Licensed under the [Apache License 2.0](LICENSE). This project is a port of [Arrow Kotlin](https://github.com/arrow-kt/arrow) `arrow-core`; upstream Arrow is also licensed under Apache 2.0.
