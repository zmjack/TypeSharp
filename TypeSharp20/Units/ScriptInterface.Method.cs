namespace TypeSharp.Units;

public partial class ScriptInterface
{
    public class Method : ScriptMethod, IAccessible, INameable
    {
        public ScriptInterface Interface { get; }

        public string Name { get; }
        public virtual QualifiedName FullName => new(Interface.FullName, Name);

        public Access Access { get; }

        public Method(Access access, string name, ScriptType @return, ScriptParameter[] parameters) : base(@return, parameters)
        {
            Access = access;
            Name = name;
        }
    }
}
