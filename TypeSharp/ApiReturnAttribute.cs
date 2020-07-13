using System;

namespace TypeSharp
{
    public class ApiReturnAttribute : Attribute
    {
        public Type ReturnType { get; set; }

        public ApiReturnAttribute(Type returnType)
        {
            ReturnType = returnType;
        }
    }

}
