using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public class ScriptVariable : ScriptVariableDefinition
    {
        public object Value { get; set; }

        public ScriptVariable(string name, ScriptType type) : base(name, type)
        {
        }

        public ScriptVariable(string name, ScriptType type, object value) : base(name, type)
        {
            Value = value;
        }

    }
}
