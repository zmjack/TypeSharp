namespace TypeSharp;

public enum Access
{
    Default,
    Public,
    Protected,
    Private,
}

public static class AccessExtensions
{
    public static string GetSnippet(this Access @this)
    {
        return @this switch
        {
            Access.Public => "public",
            Access.Protected => "protected",
            Access.Private => "private",
            _ => string.Empty,
        };
    }
}
