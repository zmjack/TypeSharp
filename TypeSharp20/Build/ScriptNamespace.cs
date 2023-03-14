using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Definitions;
using TypeSharp.Interfaces;

namespace TypeSharp.Build
{
    public class ScriptNamespace : INameable, IEncodable
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, Name);

        public ScriptNamespace[] Namespaces { get; set; }
        public ScriptClass[] Classes { get; set; }
        public ScriptInterface[] Interfaces { get; set; }
        public ScriptType[] Types { get; set; }
        public ScriptFunction[] Functions { get; set; }

        public string Encode(Indent indent, string ownerPrefix)
        {
            if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}namespace {Name}");
            sb.AppendLine($"{indent}{{");

            if (Classes is not null)
            {
                foreach (var @class in Classes)
                {
                    sb.AppendLine(@class.Encode(indent.Tab(), ownerPrefix));
                }
            }

            sb.AppendLine($"}}");

            return sb.ToString();
        }

    }
}
