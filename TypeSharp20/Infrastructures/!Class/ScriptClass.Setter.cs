using System.Collections.Generic;
using System.Text;

namespace TypeSharp.Infrastructures;

public partial record ScriptClass
{
    public class Setter : Function, INameable, IAccessible, IEncodable
    {
        public override QualifiedName FullName => new(Class?.FullName.Value, Name);

        public Setter(Access access, string name, ScriptType type, string body) : base(access, name, null, new[]
        {
            new ScriptParameter("value", type)
        }, body)
        {
        }

        public override string Encode(Indent indent, string ownerPrefix)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{indent}set {Name}(value: {Parameters[0].FullName.GetSimplifiedName(ownerPrefix)})");
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
