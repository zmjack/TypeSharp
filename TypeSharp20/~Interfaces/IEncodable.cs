using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TypeSharp
{
    public interface IEncodable
    {
        string Encode(Indent indent, string ownerPrefix);
    }
}
