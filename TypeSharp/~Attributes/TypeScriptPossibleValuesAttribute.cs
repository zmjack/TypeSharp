using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TypeScriptPossibleValuesAttribute
    {
        public string[] PossibleValues { get; set; }

        public TypeScriptPossibleValuesAttribute(string[] possiblesValues)
        {
            PossibleValues = possiblesValues;
        }

    }
}
