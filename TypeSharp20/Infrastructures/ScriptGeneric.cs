namespace TypeSharp.Infrastructures;

public class ScriptGeneric
{
    public string Name { get; set; }
    public ScriptType[] ExtendedTypes { get; set; }
    public ScriptType Default { get; set; }

    public ScriptGeneric(string name, ScriptType @default = null, ScriptType[] extends = null)
    {
        Name = name;
        ExtendedTypes = extends;
        Default = @default;
    }
}
