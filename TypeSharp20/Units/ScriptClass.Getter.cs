using System.Text;

namespace TypeSharp.Units;

public partial class ScriptClass
{
    public class Getter : Function, INameable, IAccessible, IEncodable
    {
        public override QualifiedName FullName => new(Class?.FullName, Name);

        public Getter(Access access, string name, ScriptType type, string body) : base(access, name, type, null, body)
        {
        }

        public override string Encode(Indent indent, string? ownerPrefix)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{indent}get {Name}(): {ReturnType.FullName.GetSimplifiedName(ownerPrefix)}");
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
