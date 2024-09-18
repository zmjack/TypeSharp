using System.Text;

namespace TypeSharp.Infrastructures;

public partial record ScriptInterface : IEncodable
{
    [Obsolete("", true)]
    public class Property : INameable
    {
        public ScriptInterface Interface { get; }

        public string Name { get; }
        public QualifiedName FullName => new(Interface.FullName.Value, Name);

        public Access Access { get; }
        public ScriptType Type { get; }
        public Method Get { get; }
        public Method Set { get; }

        public Property(Access access, string name, ScriptType type, Method get, Method set)
        {
            if (get is not null)
            {
                if (get.Parameters.Length != 0) throw new ArgumentException("A 'get' accessor cannot have parameters.", nameof(get));
                if (!get.ReturnType.Equals(type)) throw new InvalidOperationException($"The return type of a 'get' accessor must be {type}.");
            }

            if (set is not null)
            {
                if (set.Parameters.Length != 1) throw new ArgumentException("A 'set' accessor must have exactly one parameter.", nameof(set));
                if (!set.Parameters[0].Type.Equals(type)) throw new InvalidOperationException($"The 'set' accessor type must be {type}.");
            }

            if (get is not null && set is not null)
            {
                if (!get.ReturnType.Equals(set.Parameters[0].Type)) throw new InvalidOperationException("The return type of a 'get' accessor must be assignable to its 'set' accessor type.");
            }

            Name = name;
            Type = type;
            Access = access;
            Get = get;
            Set = set;
        }

        public string Encode(Indent indent, string ownerPrefix)
        {
            var sb = new StringBuilder();

            if (Get is not null)
            {
                sb.AppendLine($"{indent}get {Name}(): {FullName.GetSimplifiedName(ownerPrefix)})");
                sb.AppendLine($"{{");
                sb.AppendLine($"}}");
            }
            if (Set is not null)
            {
                sb.AppendLine($"{indent}set {Name}(value: {FullName.GetSimplifiedName(ownerPrefix)})");
                sb.AppendLine($"{{");
                sb.AppendLine($"}}");
            }

            return sb.ToString();
        }
    }
}
