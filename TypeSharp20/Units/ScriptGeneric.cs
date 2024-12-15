namespace TypeSharp.Units;

public class ScriptGeneric : ScriptType
{
    public ScriptType[] ExtendedTypes { get; set; }
    public ScriptType? Default { get; set; }

    public ScriptGeneric(string name)
    {
        if (string.IsNullOrEmpty(name)) throw Throws.NameIsRequired();
        Name = name;
        ExtendedTypes = [];
    }
    public ScriptGeneric(string name, ScriptType[] extends, ScriptType? @default)
    {
        if (string.IsNullOrEmpty(name)) throw Throws.NameIsRequired();
        Name = name;
        ExtendedTypes = extends ?? [];
        Default = @default;
    }
}
