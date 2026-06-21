using Arrow.Core.Raise;

var method = typeof(NullableDispatch).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
    .Where(m => m.Name.Contains("Struct"))
    .Select(m => m.Name);
Console.WriteLine(string.Join(", ", method));

try
{
    var result = RaiseBuilders.RunNullable<int>(r =>
    {
        r.Ensure(false);
        return 7;
    });
    Console.WriteLine($"Result={result}, isNull={result is null}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
}
