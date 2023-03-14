using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Definitions;

namespace TypeSharp.Interfaces
{
    public class ScriptMethod
    {
        public ScriptType Return { get; set; }
        public ScriptParameter[] Parameters { get; set; }

        public ScriptMethod(ScriptType @return, ScriptParameter[] parameters)
        {
            Return = @return ?? ScriptType.Undefined;
            Parameters = parameters ?? Array.Empty<ScriptParameter>(); ;
        }
    }
}
