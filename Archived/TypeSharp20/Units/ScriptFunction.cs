using System.Text;

namespace TypeSharp.Units;

public class ScriptFunction : ScriptMethod, INameable, IGenericGenerable
{
    public ScriptNamespace? Namespace { get; internal set; }
    public ScriptClass? Class { get; internal set; }
    public string Name { get; }
    public virtual QualifiedName FullName => new(Class?.FullName ?? Namespace?.FullName, Name);

    public ScriptType[] GenericArguments { get; set; } = [];
    public string Body { get; set; }

    public ScriptFunction(string name, ScriptType @return, ScriptParameter[] parameters, string body) : base(@return, parameters)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Body = body;
    }

    public virtual string Encode()
    {
        return Encode(Indent.Zero, Class?.FullName ?? Namespace?.FullName);
    }

    public virtual string Encode(Indent indent, string? ownerPrefix)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{indent}function {Name}({Parameters?.Select(x => x.Encode(Indent.Zero, FullName)).Join(", ")})");
        sb.AppendLine($"{indent}{{");

        foreach (var line in Body.GetPureLines())
        {
            sb.AppendLine($"{indent.Tab()}{line}");
        }
        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }
}
