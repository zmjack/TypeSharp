using NStandard;
using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptClass
    {
        public class Setter : Function, INameable, IAccessible, IEncodable
        {
            public override QualifiedName FullName => QualifiedName.Combine(Class.FullName.Value, Name);

            public Setter(ScriptClass @class, Access access, string name, ScriptType type, string body) : base(@class, access, name, null, new[]
            {
                new ScriptParameter("value", type)
            }, body)
            {
            }
        }
    }
}
