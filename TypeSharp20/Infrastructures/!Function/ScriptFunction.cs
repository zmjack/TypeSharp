using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Infrastructures;

public class ScriptFunction : ScriptMethod, INameable, IGenericGenerable
{
    public ScriptNamespace? Namespace { get; internal set; }
    public ScriptClass? Class { get; internal set; }
    public string Name { get; }
    public virtual QualifiedName FullName => new(Namespace?.Name, Name);

    public ScriptType[] GenericArguments { get; set; } = [];
    public string Body { get; set; }

    public ScriptFunction(string name, ScriptType @return, ScriptParameter[] parameters, string body) : base(@return, parameters)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Body = body;
    }

    public virtual string Encode(Indent indent, string ownerPrefix)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{indent}function {Name}({Parameters?.Select(x => x.Encode(Indent.Zero, FullName.Value)).Join(", ")})");
        sb.AppendLine($"{indent}{{");

        foreach (var line in Body.GetPureLines())
        {
            sb.AppendLine($"{indent.Tab()}{line}");
        }
        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }
}
