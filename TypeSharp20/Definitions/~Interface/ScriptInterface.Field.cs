using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
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
}
