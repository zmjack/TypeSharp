using System;

namespace TypeSharp
{
    public class ApiReturnAttribute : Attribute
    {
        public Type[] PossibleTypes { get; set; }

        public ApiReturnAttribute(params Type[] possibleTypes)
        {
            PossibleTypes = possibleTypes;
        }
    }

}
