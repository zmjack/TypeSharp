using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public class ScriptLambda : ScriptMethod
    {
        public string Body { get; set; }

        public ScriptLambda(ScriptType @return, ScriptParameter[] parameters) : base(@return, parameters)
        {
        }

        public void Script(string body)
        {
            Body = body;
        }
    }
}
