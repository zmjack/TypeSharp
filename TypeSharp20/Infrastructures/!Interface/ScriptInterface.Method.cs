using TypeSharp.Interfaces;

namespace TypeSharp.Infrastructures;

public partial record ScriptInterface
{
    public class Method : ScriptMethod, IAccessible, INameable
    {
        public ScriptInterface Interface { get; }

        public string Name { get; }
        public virtual QualifiedName FullName => new(Interface.FullName.Value, Name);

        public Access Access { get; }

        public Method(Access access, string name, ScriptType @return, ScriptParameter[] parameters) : base(@return, parameters)
        {
            Access = access;
            Name = name;
        }
    }
}
