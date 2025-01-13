namespace TypeSharp.Attributes;

public class TypeScriptPossiblesAttribute
{
    public string[] Values { get; set; }

    public TypeScriptPossiblesAttribute(string[] values)
    {
        Values = values;
    }

}
