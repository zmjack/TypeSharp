using NStandard;
using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptInterface
    {
        public class Method : ScriptMethod, IAccessible, INameable
        {
            public ScriptInterface Interface { get; }

            public string Name { get; }
            public virtual QualifiedName FullName => QualifiedName.Combine(Interface.FullName.Value, Name);

            public Access Access { get; }

            public Method(Access access, string name, ScriptType @return, ScriptParameter[] parameters) : base(@return, parameters)
            {
                Access = access;
                Name = name;
            }
        }
    }
}
