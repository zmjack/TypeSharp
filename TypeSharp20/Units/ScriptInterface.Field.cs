namespace TypeSharp.Units;

public partial class ScriptInterface
{
    public class Field : ScriptVariableDefinition, IAccessible
    {
        public ScriptInterface Interface { get; internal set; }

        public Access Access { get; }

        public Field(Access access, string name, ScriptType type) : base(name, type)
        {
            Access = access;
        }
    }
}
