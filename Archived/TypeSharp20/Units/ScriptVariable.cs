namespace TypeSharp.Units;

public class ScriptVariable : ScriptVariableDefinition
{
    public object Value { get; set; }

    public ScriptVariable(string name, ScriptType type) : base(name, type)
    {
    }

    public ScriptVariable(string name, ScriptType type, object value) : base(name, type)
    {
        Value = value;
    }

}
