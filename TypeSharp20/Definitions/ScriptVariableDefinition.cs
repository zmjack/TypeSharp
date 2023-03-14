using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp.Definitions
{
    public class ScriptVariableDefinition : INameable
    {
        public ScriptType Type { get; set; }
        public string Name { get; set; }
        public QualifiedName FullName => Name;

        public ScriptVariableDefinition(string name, ScriptType type)
        {
            Name = name;
            Type = type;
        }
    }
}


