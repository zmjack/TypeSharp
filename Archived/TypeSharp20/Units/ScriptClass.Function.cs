using System.Text;

namespace TypeSharp.Units;

public partial class ScriptClass
{
    public class Function : ScriptFunction, INameable, IAccessible, IEncodable
    {
        public override QualifiedName FullName => new(Class?.FullName, Name);

        public Access Access { get; }

        public Function(Access access, string name, ScriptType @return, ScriptParameter[] parameters, string body) : base(name, @return, parameters, body)
        {
            Access = access;
        }

        public override string Encode(Indent indent, string? ownerPrefix)
        {
            var sb = new StringBuilder();
            var declaring = $"{indent}{Access.GetSnippet()} {Name}({Parameters?.Select(x => x.Encode(Indent.Zero, FullName)).Join(", ")})";

            if (ReturnType is not null)
                sb.AppendLine($"{declaring}: {ReturnType.FullName.GetSimplifiedName(ownerPrefix)}");
            else sb.AppendLine(declaring);

            sb.AppendLine($"{indent}{{");
            foreach (var line in Body.GetPureLines())
            {
                sb.AppendLine($"{indent.Tab()}{line}");
            }
            sb.AppendLine($"{indent}}}");

            return sb.ToString();
        }
    }
}
