using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Definitions;

namespace TypeSharp
{
    public interface INameable
    {
        string Name { get; }
        QualifiedName FullName { get; }
    }
}
