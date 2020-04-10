using System;

namespace TypeSharp
{
    public class ReturnAttribute : Attribute
    {
        public Type ReturnType { get; set; }

        public ReturnAttribute(Type returnType)
        {
            ReturnType = returnType;
        }
    }

}
