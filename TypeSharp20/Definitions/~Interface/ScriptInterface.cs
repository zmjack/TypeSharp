using NStandard;
using System;
using System.Text;
using System.Threading.Tasks;
using TypeSharp.Build;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptInterface : IComment, IGenericGenerable, IEncodable, INameable
    {
        public string Namespace { get; }
        public string Name { get; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, Name);
        public ScriptGeneric[] GenericArguments { get; set; }

        public string Comment { get; set; }
        public ScriptType[] ExtendedInterfaces { get; set; }

        public Field[] Fields { get; set; } = Array.Empty<Field>();
        public Property[] Properties { get; set; } = Array.Empty<Property>();

        public ScriptInterface(string name)
        {
            Name = name;
        }

        public ScriptInterface Extends(params ScriptType[] extends)
        {
            ExtendedInterfaces = extends ?? Array.Empty<ScriptType>();
            return this;
        }

        public string Encode(Indent indent, string ownerPrefix)
        {
            if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}interface {Name}");
            sb.AppendLine($"{indent}{{");

            if (Fields is not null)
            {
                foreach (var property in Fields)
                {
                    var propertyType = property.Type.FullName.GetSimplifiedName(ownerPrefix);
                    sb.AppendLine($"{indent.Tab()}{property.Name}: {propertyType};");
                }
            }

            sb.AppendLine($"{indent}}}");

            return sb.ToString();
        }
    }
}