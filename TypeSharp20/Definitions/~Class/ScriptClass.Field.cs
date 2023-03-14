using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptClass
    {
        public class Field : ScriptVariable, IAccessible, IEncodable
        {
            public ScriptClass Class { get; }

            public Access Access { get; }

            public Field(ScriptClass @class, Access access, string name, ScriptType type) : base(name, type)
            {
                Class = @class;
                Access = access;
            }

            public Field(ScriptClass @class, Access access, string name, ScriptType type, object value) : base(name, type, value)
            {
                Class = @class;
                Access = access;
                Value = value;
            }

            public string Encode(Indent indent, string ownerPrefix)
            {
                var type = Type.FullName.GetSimplifiedName(ownerPrefix);
                return $"{indent}{Access.GetSnippet()}{Name}: {type} = undefined as any;";
            }
        }
    }
}
