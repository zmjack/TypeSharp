using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptClass
    {
        public class Property : INameable, IEncodable
        {
            public ScriptClass Class { get; }

            public string Name { get; }
            public QualifiedName FullName => QualifiedName.Combine(Class.FullName.Value, Name);

            public Access Access { get; }
            public ScriptType Type { get; }
            public Getter Getter { get; private set; }
            public Setter Setter { get; private set; }

            public Property(ScriptClass @class, Access access, string name, ScriptType type, string get = null, string set = null)
            {
                Class = @class;
                Name = name;
                Type = type;
                Access = access;
                Setter = set is not null ? new Setter(@class, Access.Public, name, type, set) : null;
            }

            public Property Get(Access access, string body)
            {
                Getter = body is not null ? new Getter(Class, access, Name, Type, body) : null;
                return this;
            }
            public Property Set(Access access, string body)
            {
                Setter = body is not null ? new Setter(Class, access, Name, Type, body) : null;
                return this;
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
}
