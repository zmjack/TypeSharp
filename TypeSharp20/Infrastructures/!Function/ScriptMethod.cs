using TypeSharp.Infrastructures;

namespace TypeSharp.Interfaces;

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
