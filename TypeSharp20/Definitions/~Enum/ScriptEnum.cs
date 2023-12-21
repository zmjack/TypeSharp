using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public partial class ScriptEnum : IDeclarable
    {
        public bool Declare { get; set; }

        public string Namespace { get; set; }
        public string Name { get; set; }
        public Field[] Fields { get; set; }
    }
}
