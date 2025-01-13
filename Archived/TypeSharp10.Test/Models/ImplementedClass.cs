using System;
using TypeSharp.Test.Models.Interfaces;

namespace TypeSharp.Test.Models
{
    internal class ImplementedClass : ISimpleInterface
    {
        public void Query()
        {
            throw new NotImplementedException();
        }
    }
}
