namespace TypeSharp.Infrastructures;

public class ScriptVariableDefinition : INameable
{
    public ScriptType Type { get; set; }
    public string Name { get; }
    public QualifiedName FullName => new(Name);

    public ScriptVariableDefinition(string name, ScriptType type)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Type = type;
    }
}


