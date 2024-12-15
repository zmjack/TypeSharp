namespace TypeSharp.DevAnalyzer.XmlParsers;

internal class Keyword
{
    public string? Name { get; set; }
    public string? ClassName { get; set; }

    public static string GetClassName(string name)
    {
        if (name.Length == 0) return "<invalid name>";

        var chars = name.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return $"{new string(chars)}Keyword";
    }
}
