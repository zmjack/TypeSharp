using NStandard;
using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial record ScriptClass
    {
        public class Getter : Function, INameable, IAccessible, IEncodable
        {
            public override QualifiedName FullName => QualifiedName.Combine(Class.FullName.Value, Name);

            public Getter(ScriptClass @class, Access access, string name, ScriptType type, string body) : base(@class, access, name, type, null, body)
            {
            }
        }
    }
}
