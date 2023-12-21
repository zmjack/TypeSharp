using NStandard;
using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial record ScriptClass
    {
        public class Function : ScriptFunction, INameable, IAccessible, IEncodable
        {
            public ScriptClass Class { get; }

            public override QualifiedName FullName => QualifiedName.Combine(Class.FullName.Value, Name);

            public Access Access { get; }

            public Function(ScriptClass @class, Access access, string name, ScriptType @return, ScriptParameter[] parameters, string body) : base(name, @return, parameters, body)
            {
                Class = @class;
                Access = access;
            }

            public new string Encode(Indent indent, string ownerPrefix)
            {
                var sb = new StringBuilder();
                var declaring = $"{indent}{Access.GetSnippet()}{Name}({Parameters?.Select(x => x.Encode(Indent.Zero, FullName.Value)).Join(", ")})";

                if (Return is not null)
                    sb.AppendLine($"{declaring}: {Return.FullName.GetSimplifiedName(ownerPrefix)}");
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
}
