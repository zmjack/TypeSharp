using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Build;

namespace TypeSharp
{
    public class CodeWriter
    {
        public List<ScriptNamespace> Namespaces = new();

        public CodeWriter(int indent = 4)
        {
        }

        public void AddNamespaces(params ScriptNamespace[] namespaces)
        {
            Namespaces.AddRange(namespaces);
        }



    }
}
