namespace TypeSharp.Infrastructures;

public class ScriptParameter : ScriptVariableDefinition, IEncodable
{
    public ScriptParameter(string name, ScriptType type) : base(name, type)
    {
    }

    public string Encode(Indent indent, string ownerPrefix)
    {
        var type = Type;
        return $"{Name}: {type.FullName.GetSimplifiedName(ownerPrefix)}";
    }
}
