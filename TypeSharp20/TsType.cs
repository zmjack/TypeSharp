using NStandard;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp
{
    public class TsType
    {
        public bool IsClass => GetType().IsType(typeof(TsClass));
    }
}
