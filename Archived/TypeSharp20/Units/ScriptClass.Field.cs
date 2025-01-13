namespace TypeSharp.Units;

public partial class ScriptClass
{
    public class Field : ScriptVariable, IAccessible, IEncodable
    {
        public ScriptClass? Class { get; internal set; }

        public Access Access { get; }

        public Field(Access access, string name, ScriptType type) : base(name, type)
        {
            Access = access;
        }

        public Field(Access access, string name, ScriptType type, object value) : base(name, type, value)
        {
            Access = access;
            Value = value;
        }

        public string Encode(Indent indent, string? ownerPrefix)
        {
            var type = Type.FullName.GetSimplifiedName(ownerPrefix);
            return $"{indent}{Access.GetSnippet()} {Name}: {type} = undefined as any;";
        }
    }
}
