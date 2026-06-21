# Arrow.Core — Full Library Guide

A .NET port of [Arrow Kotlin](https://github.com/arrow-kt/arrow) `arrow-core`, bringing functional programming abstractions and the **Raise typed-error DSL** to C#.

**Target:** .NET 10 (`net10.0`), C# preview, nullable reference types enabled.  
**Namespaces:** `Arrow.Core`, `Arrow.Core.Raise`, `Arrow.Core.Atomic`.

---

## Table of contents

1. [Getting started](#getting-started)
2. [Design conventions](#design-conventions)
3. [Option](#option)
4. [Either](#either)
5. [Ior](#ior)
6. [OperationResult](#operationresult)
7. [Raise — typed errors without exceptions](#raise--typed-errors-without-exceptions)
8. [Raise builders](#raise-builders)
9. [Effects (EagerEffect and Effect)](#effects-eagereffect-and-effect)
10. [Error accumulation](#error-accumulation)
11. [Non-empty collections](#non-empty-collections)
12. [Iterable, sequence, and map utilities](#iterable-sequence-and-map-utilities)
13. [Parallel validation (EitherZip)](#parallel-validation-eitherzip)
14. [Tuples and comparison](#tuples-and-comparison)
15. [Memoization](#memoization)
16. [Full example: validating user registration](#full-example-validating-user-registration)
17. [API quick reference](#api-quick-reference)
18. [Relation to Arrow Kotlin](#relation-to-arrow-kotlin)

---

## Getting started

### Add the project reference

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="path\to\Arrow.Core\Arrow.Core.csproj" />
  </ItemGroup>
</Project>
```

### Minimal program

```csharp
using Arrow.Core;
using Arrow.Core.Raise;

// Either: typed success or failure
Either<string, int> parsed = RaiseBuilders.RunEither<string, int>(raise =>
{
    raise.Ensure(!string.IsNullOrWhiteSpace("42"), () => "empty input");
    return int.Parse("42");
});

Console.WriteLine(parsed.Fold(
    err => $"Failed: {err}",
    n   => $"Parsed: {n}"));
// Parsed: 42
```

Run the test suite to explore more patterns:

```bash
dotnet test Arrow.sln
```

---

## Design conventions

| Convention | Meaning |
|---|---|
| **Left = error** | In `Either<Error, Success>`, `Left` holds the failure; `Right` holds the value. |
| **Bind = FlatMap** | Monadic chaining is named `Bind` (Kotlin/Arrow style) and aliases `FlatMap`. |
| **Raise short-circuits** | Calling `raise.Raise(error)` aborts the block and maps the error into the result type. |
| **Non-fatal exceptions** | Real exceptions propagate unless caught via `Catch` / `Recover`; fatal exceptions always rethrow. |

Factory helpers follow a consistent style:

```csharp
EitherExtensions.Left<string, int>("error");
EitherExtensions.Right<string, int>(42);
OptionExtensions.Some(1);
OptionExtensions.None<int>();
IorExtensions.Left<string, int>("warn");
IorExtensions.Right<string, int>(1);
IorExtensions.Both("warn", 1);
```

---

## Option

`Option<A>` models an optional value: **Some** or **None**.

### Construction

```csharp
using Arrow.Core;

Option<int> fromValue  = OptionExtensions.Some(42);
Option<int> empty      = OptionExtensions.None<int>();
Option<string> fromNull = Option<string>.FromNullable(null);       // None
Option<string> fromStr  = Option<string>.FromNullable("hello");  // Some("hello")
Option<string> ext      = "hello".ToOption();                    // extension
```

### Transform and query

```csharp
Option<int> doubled = OptionExtensions.Some(21)
    .Map(n => n * 2)                          // Some(42)
    .Filter(n => n > 0);                      // Some(42)

int value = OptionExtensions.Some(42)
    .GetOrElse(() => -1);                     // 42

Option<int> none = OptionExtensions.None<int>()
    .GetOrElse(() => -1);                     // -1 (via extension on None)
```

### Monadic bind (chaining)

```csharp
Option<string> parsePositive(string input) =>
    int.TryParse(input, out var n) && n > 0
        ? OptionExtensions.Some(n.ToString())
        : OptionExtensions.None<string>();

Option<int> result = OptionExtensions.Some("21")
    .Bind(parsePositive)
    .Map(s => s.Length);                      // Some(2)
```

### Convert to Either

```csharp
Either<string, int> either = OptionExtensions.Some(42)
    .ToEither(() => "missing value");        // Right(42)

Either<string, int> left = OptionExtensions.None<int>()
    .ToEither(() => "missing value");         // Left("missing value")
```

---

## Either

`Either<A, B>` is a sum type: **Left(A)** or **Right(B)**. In error-handling code, **A is the error type** and **B is the success type**.

### Construction and fold

```csharp
Either<string, int> success = EitherExtensions.Right<string, int>(42);
Either<string, int> failure = EitherExtensions.Left<string, int>("not found");

string message = success.Fold(
    err => $"Error: {err}",
    n   => $"Got {n}");                       // "Got 42"
```

### Map and flatMap

```csharp
Either<string, int> doubled = EitherExtensions.Right<string, int>(21)
    .Map(n => n * 2);                         // Right(42)

Either<string, int> chained = EitherExtensions.Right<string, int>("21")
    .FlatMap(s => int.TryParse(s, out var n)
        ? EitherExtensions.Right<string, int>(n)
        : EitherExtensions.Left<string, int>("parse error"));
```

### Catch exceptions

```csharp
Either<Exception, int> safe = Either<Exception, int>.Catch(() =>
{
    return int.Parse("not-a-number");
});
// Left(FormatException)

Either<FormatException, int> typed = Either<FormatException, int>.CatchOrThrow(() =>
    int.Parse("not-a-number"));
```

### Get values safely

```csharp
int? value = EitherExtensions.Right<string, int>(42).GetOrNull();   // 42
string? err = EitherExtensions.Left<string, int>("x").LeftOrNull(); // "x"
Option<int> opt = EitherExtensions.Right<string, int>(42).GetOrNone(); // Some(42)
```

### Combine errors (monoid)

When `A` has a combine function, multiple `Either<A, B>` values can merge:

```csharp
Either<string, int> combined = EitherExtensions.Combine(
    string.Concat,
    EitherExtensions.Left<string, int>("a"),
    EitherExtensions.Left<string, int>("b"));
// Left("ab")
```

---

## Ior

`Ior<A, B>` (inclusive-or) has three cases:

- **Left(A)** — only the left value (often a warning/error)
- **Right(B)** — only the right value (success)
- **Both(A, B)** — both values present (partial success with warnings)

### Construction

```csharp
Ior<string, int> onlyError  = IorExtensions.Left<string, int>("warning");
Ior<string, int> onlyValue  = IorExtensions.Right<string, int>(42);
Ior<string, int> both       = IorExtensions.Both("deprecated API", 42);

Ior<string, int>? fromNulls = Ior<string, int>.FromNullables(null, 42); // Right(42)
```

### Fold and map

```csharp
string description = IorExtensions.Both("slow query", 100).Fold(
    left  => $"Failed: {left}",
    right => $"Success: {right}",
    both  => (l, r) => $"Partial ({l}): {r}");
// "Partial (slow query): 100"

Ior<string, string> mapped = IorExtensions.Both("warn", 42)
    .Map(n => n.ToString());                  // Both("warn", "42")
```

### FlatMap requires a combine function

When chaining `Ior`, left values from successive steps must be merged:

```csharp
Ior<string, int> result = IorExtensions.Both("step1", 1)
    .FlatMap(string.Concat, n => IorExtensions.Both(", step2", n + 1));
// Both("step1, step2", 2)
```

---

## OperationResult

Kotlin-style `Result` (distinct from `System.Result<T>`):

```csharp
using Arrow.Core;

OperationResult<int> ok  = OperationResult<int>.Success(42);
OperationResult<int> bad = OperationResult<int>.Failure(new InvalidOperationException("boom"));

int value = ok.Match(
    onFailure: ex => throw ex,
    onSuccess: v  => v);                      // 42

OperationResult<string> mapped = ok.FlatMap(n =>
    OperationResult<string>.Success(n.ToString()));
```

Used by **`RaiseBuilders.RunResult`** (see below).

---

## Raise — typed errors without exceptions

**Raise** is Arrow's DSL for typed, short-circuiting control flow. Instead of throwing for expected failures, you call `raise.Raise(error)` and the builder converts that into `Either.Left`, `Option.None`, `null`, etc.

Core interface:

```csharp
namespace Arrow.Core.Raise;

public interface IRaise<in Error>
{
    void Raise(Error error);  // short-circuits the computation
}
```

### Bind Either values inside Raise

```csharp
using Arrow.Core;
using Arrow.Core.Raise;

Either<string, int> result = RaiseBuilders.RunEither<string, int>(raise =>
{
    // Fail fast with a custom error
    raise.Ensure(age >= 0, () => "age must be non-negative");

    // Unwrap an Either — Left short-circuits the whole block
    var name = raise.Bind(parseName(input));   // Either<string, string>
    return name.Length;
});
```

Available on `IRaise<Error>`:

| Method | Description |
|---|---|
| `Bind(Either<Error, A>)` | Return `A` or short-circuit on `Left` |
| `BindAll(...)` | Bind over lists, maps, `NonEmptyList`, `NonEmptySet` |
| `Ensure(condition, () => error)` | Raise if condition is false |
| `EnsureNotNull(value, () => error)` | Raise if reference is null |
| `Recover(block, recover)` | Catch raised errors in a nested scope |
| `WithError(transform, block)` | Change the error type in a nested scope |

---

## Raise builders

Each builder runs a `Raise` block and materializes a concrete result type.

### Either — `RunEither`

```csharp
Either<string, User> RegisterUser(string raw) =>
    RaiseBuilders.RunEither<string, User>(raise =>
    {
        raise.Ensure(!string.IsNullOrWhiteSpace(raw), () => "empty input");

        var email = raise.Bind(ParseEmail(raw));
        var age   = raise.Bind(ParseAge(raw));

        return new User(email, age);
    });

static Either<string, string> ParseEmail(string input) =>
    input.Contains('@')
        ? EitherExtensions.Right<string, string>(input)
        : EitherExtensions.Left<string, string>("invalid email");

static Either<string, int> ParseAge(string input) =>
    int.TryParse(input, out var n) && n >= 0
        ? EitherExtensions.Right<string, int>(n)
        : EitherExtensions.Left<string, int>("invalid age");

record User(string Email, int Age);
```

### Nullable — `RunNullable`

Logical failure becomes `null` (uses `SingletonRaise` internally):

```csharp
int? length = RaiseBuilders.RunNullable(r =>
    r.BindNullable(getNullableString())?.Length ?? throw new InvalidOperationException());

// Ensure / EnsureNotNull also short-circuit to null
int? safe = RaiseBuilders.RunNullable<int>(r =>
{
    r.Ensure(isValid);
    return r.EnsureNotNull(maybeValue);
});
```

`SingletonRaise` also supports:

- `Bind(Option<A>)`, `BindNullable(A?)` where `A : class`
- `BindAllOption`, `BindAllNullable` over collections
- `Recover(inner, onRaise)` — local recovery

### Option — `RunOption`

Logical failure becomes `Option.None`:

```csharp
Option<int> opt = RaiseBuilders.RunOption(r =>
{
    r.Ensure(password.Length >= 8);
    var hash = r.Bind(HashPassword(password));  // Option<string>
    return hash.Length;
});
// Some(n) or None
```

### Result — `RunResult`

Failures become `OperationResult.Failure`; `Bind(OperationResult<T>)` unwraps success:

```csharp
OperationResult<int> result = RaiseBuilders.RunResult(raise =>
{
    var step = raise.Bind(OperationResult<int>.Success(1));
    return step + 1;
});
// Success(2)

OperationResult<int> failed = RaiseBuilders.RunResult(raise =>
{
    raise.Raise(new InvalidOperationException("boom"));
    return 0;
});
// Failure(InvalidOperationException)
```

### Ior — `RunIor` / `RunIorNel`

Accumulates non-fatal errors while continuing execution. Requires an error **combine** function:

```csharp
Ior<string, int> result = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
{
    var one = r.Bind(IorExtensions.Both("Hello", 1));
    var two = r.Bind(IorExtensions.Both(", World!", 2));
    return one + two;
});
// Ior.Both("Hello, World!", 3)

// Short-circuit on Left
Ior<string, int> failed = RaiseBuilders.RunIor<string, int>(string.Concat, r =>
{
    r.Bind(IorExtensions.Both("Hello", 1));
    r.Bind(IorExtensions.Left<string, int>(", World!"));
    return 0;
});
// Ior.Left("Hello, World!")
```

`IorRaise<Error>` additionally provides:

- `Bind(Ior<Error, A>)` — merge `Both` left parts, short-circuit on `Left`
- `GetOrAccumulate(Either<Error, A>, recover)` — accumulate `Either.Left` as `Both`
- `Accumulate(error)` — manually record a non-fatal error
- `BindAllIor(...)` — bind over collections of `Ior`
- `Recover(block, recover)` — nested recovery with error merging

### Impure — side effects

```csharp
RaiseBuilders.Impure(r =>
{
    r.Ensure(fileExists);
    File.WriteAllText(path, content);
});
```

---

## Effects (EagerEffect and Effect)

Effects package Raise blocks as **reusable, composable values** without running them immediately.

```csharp
using Arrow.Core.Raise;

// Synchronous
EagerEffect<string, int> parseInt = RaiseEffect.Eager<string, int>(raise =>
    raise.Bind(ParseStrict(input)));

// Asynchronous
Effect<string, User> loadUser = RaiseEffect.Of<string, User>(async raise =>
{
    var json = await http.GetStringAsync(url);
    return raise.Bind(ParseUser(json));
});
```

### Running effects

```csharp
// Fold into any type
int value = parseInt.Fold(
    recover: err => -1,
    transform: n => n);

// Map to Either / Ior / Option / Result
Either<string, int> either = parseInt.ToEither();
Ior<string, int> ior       = parseInt.ToIor();
Option<int> option         = parseInt.ToOption(_ => OptionExtensions.None<int>());
OperationResult<int> res   = parseInt.ToResult(_ => OperationResult<int>.Success(-1));

// Async variants
Either<string, User> user = await loadUser.ToEitherAsync();
```

### Recover, catch, and mapError

Transform errors or recover from them **without** re-entering a builder:

```csharp
EagerEffect<string, int> effect = RaiseEffect.Eager<string, int>(r =>
{
    r.Raise("original");
    return 0;
});

// Change error type and recover
EagerEffect<string, int> recovered = effect
    .Recover<string, string, int>((_, _) => 42);

Either<string, int> right = recovered.ToEither();  // Right(42)

// Catch exceptions
EagerEffect<int, int> caught = RaiseEffect.Eager<int, int>(_ =>
        throw new InvalidOperationException("boom"))
    .Catch((_, _) => 99);

// Map error type
EagerEffect<int, int> mapped = RaiseEffect.Eager<int, int>(r => r.Raise(404))
    .MapError<int, string, int>(code => $"HTTP {code}");
```

---

## Error accumulation

When you need **all validation errors** (not just the first), use **`NonEmptyList<Error>`** as the error type and accumulate extensions.

Marked **`[ExperimentalRaiseAccumulateApi]`**.

### ZipOrAccumulate — parallel validation

```csharp
using Arrow.Core;
using Arrow.Core.Raise;

Either<NonEmptyList<string>, Registration> result =
    RaiseBuilders.RunEither<NonEmptyList<string>, Registration>(raise =>
        raise.ZipOrAccumulate(
            acc => acc.Bind(validateEmail(rawEmail)),
            acc => acc.Bind(validatePassword(rawPassword)),
            acc => acc.Bind(validateAge(rawAge)),
            (email, password, age) => new Registration(email, password, age)));

// If all three fail → Left(NonEmptyList["bad email", "weak password", "invalid age"])
```

### MapOrAccumulate — accumulate over a collection

```csharp
Either<NonEmptyList<string>, List<ParsedField>> parsed =
    RaiseBuilders.RunEither<NonEmptyList<string>, List<ParsedField>>(raise =>
        raise.MapOrAccumulate(
            fields,
            (acc, field) => acc.Bind(parseField(field))));
```

### Accumulating — tolerant sub-blocks

```csharp
Either<NonEmptyList<string>, int> result =
    RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
        RaiseBuilders.RunEither<NonEmptyList<string>, int>,
        acc =>
        {
            _ = acc.Accumulating(inner => { inner.Raise("error 1"); return 1; });
            _ = acc.Accumulating(inner => { inner.Raise("error 2"); return 2; });
            return 3;
        });
// Left(["error 1", "error 2"])
```

### EitherZip (without Raise)

Parallel validation on plain `Either` values:

```csharp
using Arrow.Core;

var combined = EitherZip.ZipOrAccumulate(
    combine: string.Concat,
    EitherExtensions.Left<string, int>("a"),
    EitherExtensions.Left<string, int>("b"),
    (x, y) => x + y);
// Left("ab")
```

Overloads exist for 2–10 arguments, with `NonEmptyList<E>` error variants.

---

## Non-empty collections

Types that **guarantee at least one element**, mirroring Kotlin's `Nel`.

### NonEmptyList

```csharp
NonEmptyList<int> nel = Nel.Of(1, 2, 3);
NonEmptyList<int> single = Nel.Of(42);
NonEmptyList<int> fromList = NonEmptyList<int>.FromList(new[] { 1, 2 });

int head = nel.Head;                                    // 1
IReadOnlyList<int> tail = nel.Tail;                     // [2, 3]

NonEmptyList<string> mapped = nel.Map(n => n.ToString());
NonEmptyList<(int, int)> zipped = nel.Zip(Nel.Of(10, 20, 30));
```

Safe construction from possibly-empty input:

```csharp
List<int> maybeEmpty = new();
NonEmptyList<int>? nel = maybeEmpty.ToNonEmptyListOrNull();   // null
Option<NonEmptyList<int>> opt = maybeEmpty.ToNonEmptyListOrNone(); // None
```

### NonEmptySet

```csharp
NonEmptySet<string> nes = NonEmptySet<string>.Of("a", "b", "c");
NonEmptySet<int> mapped = nes.Map(n => n * 2);
```

Both types implement **`INonEmptyCollection<E>`** with `Map`, `FlatMap`, `Zip`, `Plus`, `Distinct`, and conversions to `List` / `IEnumerable`.

---

## Iterable, sequence, and map utilities

Arrow provides **align**, **padZip**, **zip** (up to 10 sequences), **crosswalk**, **filterOption**, and **mapOrAccumulate** in three flavors:

| API | Return type | Use when |
|---|---|---|
| `IterableExtensions` | `List<T>` | Eager, known-size collection processing |
| `SequenceExtensions` | `IEnumerable<T>` | Lazy, possibly infinite streams |
| `MapExtensions` | `Dictionary<K,V>` | Key-aligned operations on dictionaries |

### Zip

```csharp
List<(int, string)> pairs = IterableExtensions.Zip(
    new[] { 1, 2, 3 },
    new[] { "a", "b", "c" },
    (n, s) => (n, s));
// [(1,"a"), (2,"b"), (3,"c")]
```

### PadZip — zip with padding

When sequences differ in length, pad the shorter side with `null`:

```csharp
List<(int?, string)> leftPadded = IterableExtensions.LeftPadZip(
    new List<int> { 1 },
    new List<string> { "a", "b", "c" });
// [(1,"a"), (null,"b"), (null,"c")]

List<(int, string?)> rightPadded = IterableExtensions.RightPadZip(
    new List<int> { 1, 2, 3 },
    new List<string> { "a" });
// [(1,"a"), (2,null), (3,null)]
```

### Align — merge by position into Ior

```csharp
List<Ior<int, string>> aligned = IterableExtensions.Align(
    new[] { 1, 2 },
    new[] { "a", "b", "c" });
// [Both(1,"a"), Both(2,"b"), Right("c")]
```

### MapOrAccumulate on iterables

```csharp
Either<string, List<int>> result = IterableExtensions.MapOrAccumulate<int, string, int>(
    new[] { 1, 2, 3, 4 },
    combine: string.Concat,
    transform: (raise, value) =>
    {
        if (value % 2 == 0) return value;
        raise.Raise($"odd:{value}");
        return value;
    });
// Left("odd:1odd:3") or Right([1,2,3,4]) if all pass
```

### Dictionary zip / align

```csharp
Dictionary<string, int> left  = new() { ["x"] = 1, ["y"] = 2 };
Dictionary<string, int> right = new() { ["x"] = 10, ["z"] = 30 };

Dictionary<string, Ior<int, int>> aligned = MapExtensions.Align(left, right);
// x → Both(1,10), y → Left(2), z → Right(30)
```

---

## Parallel validation (EitherZip)

Validate multiple independent inputs and combine all errors:

```csharp
Either<string, string> validateForm(string name, int age, string email) =>
    EitherZip.ZipOrAccumulate(
        string.Concat,
        ValidateName(name),
        ValidateAge(age),
        ValidateEmail(email),
        (n, a, e) => $"{n} ({a}) — {e}");

static Either<string, string> ValidateName(string name) =>
    name.Length >= 2
        ? EitherExtensions.Right<string, string>(name)
        : EitherExtensions.Left<string, string>("name too short");
```

For multiple errors of the same type collected into a list, use the `NonEmptyList<E>` overloads.

---

## Tuples and comparison

### Extended tuples

Named structs `Tuple4` through `Tuple9` plus `CompareTo` for lexicographic ordering:

```csharp
var t = new Tuple4<int, string, bool, double>(1, "a", true, 3.14);
int cmp = t.CompareTo(new Tuple4<int, string, bool, double>(1, "b", true, 0));
```

Built-in `(A,B)` and `(A,B,C)` tuples support chaining via `TupleNExtensions.Plus`:

```csharp
var triple = (1, "a").Plus(true);   // (1, "a", true)
```

### Comparison helpers

```csharp
(int First, int Second, int Third) sorted = Comparison.Sort(3, 1, 2);  // (1, 2, 3)
List<int> sortedList = Comparison.Sort(3, 1, 2);                       // [1, 2, 3]
```

---

## Memoization

Memoize deep recursive functions by argument:

```csharp
using Arrow.Core;

var fib = MemoizedDeepRecursiveFunction<int, long>.Create((self, n) =>
    n switch
    {
        <= 1 => n,
        _    => self.Invoke(n - 1) + self.Invoke(n - 2)
    });

long f40 = fib.Invoke(40);   // fast — each n computed once
```

Custom cache:

```csharp
var cache = new AtomicMemoizationCache<string, int>();
var fn = new MemoizedDeepRecursiveFunction<string, int>(
    cache,
    (self, key) => /* ... */);
```

---

## Full example: validating user registration

This example ties together **Either**, **Raise**, **Effects**, **accumulation**, and **NonEmptyList**.

```csharp
using Arrow.Core;
using Arrow.Core.Raise;

// ── Domain types ──────────────────────────────────────────────

record Email(string Value);
record Password(string Value);
record Age(int Value);
record User(Email Email, Password Password, Age Age);

// ── Validation helpers returning Either ───────────────────────

static Either<string, Email> ParseEmail(string raw) =>
    raw.Contains('@') && raw.Length >= 5
        ? EitherExtensions.Right<string, Email>(new Email(raw.Trim().ToLowerInvariant()))
        : EitherExtensions.Left<string, Email>("invalid email");

static Either<string, Password> ParsePassword(string raw) =>
    raw.Length >= 8
        ? EitherExtensions.Right<string, Password>(new Password(raw))
        : EitherExtensions.Left<string, Password>("password must be at least 8 characters");

static Either<string, Age> ParseAge(string raw) =>
    int.TryParse(raw, out var n) && n is >= 0 and <= 150
        ? EitherExtensions.Right<string, Age>(new Age(n))
        : EitherExtensions.Left<string, Age>("invalid age");

// ── 1. Simple Either builder (fail-fast) ──────────────────────

static Either<string, User> RegisterFailFast(string emailRaw, string passwordRaw, string ageRaw) =>
    RaiseBuilders.RunEither<string, User>(raise =>
    {
        var email    = raise.Bind(ParseEmail(emailRaw));
        var password = raise.Bind(ParsePassword(passwordRaw));
        var age      = raise.Bind(ParseAge(ageRaw));
        return new User(email, password, age);
    });

// ── 2. Accumulate all validation errors ───────────────────────

static Either<NonEmptyList<string>, User> RegisterAccumulating(
    string emailRaw, string passwordRaw, string ageRaw) =>
    RaiseBuilders.RunEither<NonEmptyList<string>, User>(raise =>
        raise.ZipOrAccumulate(
            acc => acc.Bind(ParseEmail(emailRaw)),
            acc => acc.Bind(ParsePassword(passwordRaw)),
            acc => acc.Bind(ParseAge(ageRaw)),
            (email, password, age) => new User(email, password, age)));

// ── 3. Reusable Effect (can be composed, recovered, mapped) ───

static EagerEffect<string, User> RegisterEffect(string emailRaw, string passwordRaw, string ageRaw) =>
    RaiseEffect.Eager<string, User>(raise =>
    {
        var email    = raise.Bind(ParseEmail(emailRaw));
        var password = raise.Bind(ParsePassword(passwordRaw));
        var age      = raise.Bind(ParseAge(ageRaw));
        return new User(email, password, age);
    });

// ── 4. Usage ──────────────────────────────────────────────────

void Demo()
{
    // Fail-fast: first error stops validation
    var fast = RegisterFailFast("bad", "short", "999");
    Console.WriteLine(fast.Fold(e => $"Rejected: {e}", u => $"Welcome {u.Email.Value}"));

    // Accumulate: collect every validation error
    var all = RegisterAccumulating("bad", "short", "999");
    all.Fold(
        errors => Console.WriteLine($"Errors: {string.Join(", ", errors.All)}"),
        user   => Console.WriteLine($"Welcome {user.Email.Value}"));

    // Effect with recovery
    var recovered = RegisterEffect("bad", "short", "999")
        .Recover<string, string, User>((_, err) =>
            new User(new Email("guest@example.com"), new Password("guestpass"), new Age(0)));

    Either<string, User> either = recovered.ToEither();
    Console.WriteLine(either.Fold(
        e => $"Still failed: {e}",
        u => $"Recovered guest: {u.Email.Value}"));

    // Ior: warnings + success (e.g. deprecated field used but registration proceeds)
    Ior<string, User> partial = RaiseBuilders.RunIor<string, User>(string.Concat, r =>
    {
        var email = r.Bind(IorExtensions.Both("deprecated: old API", ParseEmail("user@example.com").Fold(
            e => throw new InvalidOperationException(e),
            x => x)));
        var password = r.Bind(ParsePassword("securepass123").ToIor());
        var age = r.Bind(ParseAge("30").ToIor());
        return new User(email, password, age);
    });

    partial.Fold(
        err  => Console.WriteLine($"Failed: {err}"),
        user => Console.WriteLine($"OK: {user.Email.Value}"),
        (warn, user) => Console.WriteLine($"OK with warnings [{warn}]: {user.Email.Value}"));
}
```

Expected output for invalid input:

```
Rejected: invalid email
Errors: invalid email, password must be at least 8 characters, invalid age
Recovered guest: guest@example.com
```

---

## API quick reference

### RaiseBuilders

| Method | Result type |
|---|---|
| `RunEither<Error, A>(block)` | `Either<Error, A>` |
| `RunNullable<A>(block)` | `A?` |
| `RunOption<A>(block)` | `Option<A>` |
| `RunResult<A>(block)` | `OperationResult<A>` |
| `RunIor<Error, A>(combine, block)` | `Ior<Error, A>` |
| `RunIorNel<Error, A>(combine?, block)` | `Ior<NonEmptyList<Error>, A>` |
| `Singleton(onRaise, block)` | `A` |
| `Impure(block)` | `void` |

### Effect mappers (`RaiseMappers`)

| Extension | Returns |
|---|---|
| `ToEither()` / `ToEitherAsync()` | `Either<Error, A>` |
| `ToIor()` / `ToIorAsync()` | `Ior<Error, A>` |
| `ToOption(recover)` | `Option<A>` |
| `ToResult(...)` | `OperationResult<A>` |
| `GetOrElse(recover)` / `GetOrElseAsync(...)` | `A` |

### Collection entry points

| Class | Extension target | Materialization |
|---|---|---|
| `IterableExtensions` | `IEnumerable<T>` | `List<T>` |
| `SequenceExtensions` | `IEnumerable<T>` | `IEnumerable<T>` (lazy) |
| `MapExtensions` | `IReadOnlyDictionary<K,V>` | `Dictionary<K,V>` |
| `NonEmptyList` / `NonEmptySet` | — | non-empty wrappers |

---

## Relation to Arrow Kotlin

| Kotlin (arrow-core) | Arrow.Core (.NET) |
|---|---|
| `option { }` | `RaiseBuilders.RunOption` |
| `either { }` | `RaiseBuilders.RunEither` |
| `nullable { }` | `RaiseBuilders.RunNullable` |
| `result { }` | `RaiseBuilders.RunResult` |
| `ior { }` | `RaiseBuilders.RunIor` |
| `iorNel { }` | `RaiseBuilders.RunIorNel` |
| `accumulate { }` | `RaiseAccumulateExtensions.Accumulate` + `ZipOrAccumulate` |
| `Effect` / `EagerEffect` | `Effect<Error,A>` / `EagerEffect<Error,A>` |
| `Nel` | `NonEmptyList<E>` / `Nel.Of(...)` |
| `EitherNel` | `Either<NonEmptyList<E>, A>` |
| `raise()` | `IRaise<Error>.Raise(error)` |
| `bind()` | `raise.Bind(either)` / `.FlatMap()` |

For deeper background on Raise and typed errors, see the [Arrow documentation](https://arrow-kt.io/learn/typed-errors/working-with-typed-errors/).

---

## Further reading

- Source tests in `src/core/Arrow.Core.Test/` demonstrate property-tested laws and integration patterns.
- Experimental APIs: `[ExperimentalRaiseAccumulateApi]`, `[ExperimentalTraceApi]` (see `Raise/Trace.cs`).
- Obsolete APIs include `SequenceExtensions.Unweave` and niche Either helpers marked via `EitherExtensions.NicheApi`.
