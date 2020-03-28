using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsType
    {
        /// <summary>
        /// Hint: If Namespace is null, the properties should be null.
        /// </summary>
        public string Namespace { get; set; }

        public string TypeName { get; set; }

        public TsTypeClass TypeClass { get; set; }

        public TsProperty[] TsProperties { get; set; }

        public TsEnumValue[] TsEnumValues { get; set; }

        public string ReferenceName => Namespace is null ? TypeName : $"{Namespace}.{TypeName}";
    }
}
