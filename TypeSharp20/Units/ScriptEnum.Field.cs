namespace TypeSharp.Units;

public partial class ScriptEnum : IDeclarable
{
    public class Field : ScriptVariable
    {
        public ScriptEnum? Enum { get; internal set; }

        public Field(ScriptType type, string name) : base(name, type)
        {
        }
    }
}
