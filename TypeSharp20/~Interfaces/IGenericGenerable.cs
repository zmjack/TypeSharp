﻿using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Definitions;

namespace TypeSharp
{
    public interface IGenericGenerable
    {
        ScriptType[] GenericArguments { get; set; }
    }
}
