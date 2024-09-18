using TypeSharp.Interfaces;

namespace TypeSharp.Infrastructures;

public class ScriptLambda : ScriptMethod
{
    public string Body { get; set; }

    public ScriptLambda(ScriptType @return, ScriptParameter[] parameters) : base(@return, parameters)
    {
    }

    public void Script(string body)
    {
        Body = body;
    }
}
