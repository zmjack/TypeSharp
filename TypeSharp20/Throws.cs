namespace TypeSharp;

internal class Throws
{
    internal static InvalidOperationException DuplicateEntriesAreNotAllowed() => new("Duplicate entries are not allowed.");
    internal static InvalidOperationException NameIsRequired() => new("Name is required.");
    internal static InvalidOperationException TypeMismatched() => new("Type mismatched.");
}
