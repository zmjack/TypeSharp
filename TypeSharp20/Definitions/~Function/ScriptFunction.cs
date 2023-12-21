using NStandard;
using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Build;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public class ScriptFunction : ScriptMethod, INameable, IGenericGenerable
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public virtual QualifiedName FullName => QualifiedName.Combine(Namespace, Name);

        public ScriptType[] GenericArguments { get; set; } = Array.Empty<ScriptType>();
        public string Body { get; set; }

        public ScriptFunction(string name, ScriptType @return, ScriptParameter[] parameters, string body) : base(@return, parameters)
        {
            Name = name;
            Body = body;
        }

        public string Encode(Indent indent, string ownerPrefix)
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
}
