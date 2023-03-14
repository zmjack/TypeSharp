using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp.Definitions
{
    public enum ScriptTypeType
    {
        None = 0,

        BuiltInType = 0x1,
        Type = 0x2,

        Interface = 0x4,
        Enum = 0x8,

        Class = 0x10,
        Method = 0x20,

        Generic = 0x1000,
    }
}
