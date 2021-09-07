namespace TypeSharp.Test
{
    [TypeScriptModel]
    public class WapperClass
    {
        [TypeScriptModel]
        public class NestedClass
        {
            public int IntValue { get; set; }
        }

        public NestedClass Nested { get; set; }
    }
}
