using NStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeSharp.Build;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptClass : IDeclarable, INameable, IGenericGenerable, IEncodable
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, Name);
        public ScriptGeneric[] GenericArguments { get; set; } = Array.Empty<ScriptGeneric>();

        public bool Declare { get; set; }

        public ScriptClass InheritedClass { get; set; }
        public ScriptType[] ImplementedTypes { get; set; } = Array.Empty<ScriptType>();

        public Field[] Fields { get; set; } = Array.Empty<Field>();
        public Property[] Properties { get; set; } = Array.Empty<Property>();
        public Function[] Functions { get; set; } = Array.Empty<Function>();

        public ScriptClass(string name)
        {
            Name = name;
        }

        public ScriptClass Extends(ScriptClass extends)
        {
            InheritedClass = extends;
            return this;
        }

        public ScriptClass Implements(params ScriptType[] implements)
        {
            ImplementedTypes = implements ?? Array.Empty<ScriptType>(); ;
            return this;
        }

        public string Encode(Indent indent, string ownerPrefix)
        {
            if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

            var inheritedClass = InheritedClass?.FullName.GetSimplifiedName(ownerPrefix);
            var implements = ImplementedTypes?.Select(x => x.FullName.GetSimplifiedName(ownerPrefix));

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}class {Name}{(inheritedClass is not null ? $" {inheritedClass}" : string.Empty)}{(implements is not null ? $" implements {implements.Join(", ")}" : string.Empty)}");
            sb.AppendLine($"{indent}{{");

            if (Fields is not null)
            {
                foreach (var field in Fields)
                {
                    var code = field.Encode(indent.Tab(), FullName.Value);
                    sb.AppendLine(code);
                }
                if (Fields.Any()) sb.AppendLine();
            }
            if (Properties is not null)
            {
                foreach (var property in Properties)
                {
                    var code = property.Encode(indent.Tab(), FullName.Value);
                    sb.AppendLine(code);
                }
            }
            if (Functions is not null)
            {
                foreach (var function in Functions)
                {
                    var code = function.Encode(indent.Tab(), FullName.Value);
                    sb.AppendLine(code);
                }
            }

            sb.AppendLine($"{indent}}}");

            return sb.ToString();
        }

    }
}
