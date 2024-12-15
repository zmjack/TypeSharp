namespace TypeSharp.Units;

public class ScriptMethod
{
    public ScriptType ReturnType { get; set; }
    public ScriptParameter[] Parameters { get; set; }

    public ScriptMethod(ScriptType @return, ScriptParameter[] parameters)
    {
        ReturnType = @return ?? ScriptType.Undefined;
        Parameters = parameters ?? [];
    }
}
