using System.Text;

namespace TypeSharp.Infrastructures;

public partial record ScriptClass
{
    public class Property : INameable, IEncodable
    {
        public ScriptClass? Class { get; internal set; }

        public string Name { get; }
        public QualifiedName FullName => new(Class?.FullName.Value, Name);
        public ScriptType Type { get; }

        private Getter _getter;
        public Getter Getter
        {
            get => _getter;
            set
            {
                if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
                if (value.ReturnType != Type) throw Throws.TypeMismatched();
                value.Class = Class;
                _getter = value;
            }
        }

        private Setter _setter;
        public Setter Setter
        {
            get => _setter;
            set
            {
                if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
                if (value.ReturnType != Type) throw Throws.TypeMismatched();
                value.Class = Class;
                _setter = value;
            }
        }

        public Property(string name, ScriptType type)
        {
            Name = name;
            Type = type;
        }

        public string Encode(Indent indent, string ownerPrefix)
        {
            var sb = new StringBuilder();

            if (Getter is not null)
            {
                var declaring = $"{indent}get {Name}(): {Type.FullName.GetSimplifiedName(ownerPrefix)}";
                if (Getter.Body is not null)
                {
                    sb.AppendLine(declaring);
                    sb.AppendLine($"{indent}{{");
                    foreach (var line in Getter.Body.GetPureLines())
                    {
                        sb.AppendLine($"{indent.Tab()}{line}");
                    }
                    sb.AppendLine($"{indent}}}");
                }
                else sb.AppendLine($"{declaring} {{ }}");
            }

            if (Setter is not null)
            {
                var declaring = $"{indent}set {Name}(value: {Type.FullName.GetSimplifiedName(ownerPrefix)})";
                if (Setter.Body is not null)
                {
                    sb.AppendLine(declaring);
                    sb.AppendLine($"{indent}{{");
                    foreach (var line in Setter.Body.GetPureLines())
                    {
                        sb.AppendLine($"{indent.Tab()}{line}");
                    }
                    sb.AppendLine($"{indent}}}");
                }
                else sb.AppendLine($"{declaring} {{ }}");
            }

            return sb.ToString();
        }
    }
}
