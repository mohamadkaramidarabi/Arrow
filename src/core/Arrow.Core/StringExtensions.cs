namespace Arrow.Core;

public static class StringExtensions
{
    public static string Escaped(this string value) =>
        value
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\"", "\\\"")
            .Replace("\'", "\\\'")
            .Replace("\t", "\\t")
            .Replace("\b", "\\b");
}
