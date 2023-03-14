using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptEnum : IDeclarable
    {
        public class Field : ScriptVariable
        {
            public Field(ScriptType type, string name) : base(name, type)
            {
            }
        }
    }
}
