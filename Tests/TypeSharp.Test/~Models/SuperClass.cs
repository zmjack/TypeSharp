namespace TypeSharp.Test
{
    [TypeScriptModel]
    public class SuperClass
    {
        [TypeScriptModel]
        public class NestedClass
        {
            public int IntValue { get; set; }
        }

        public NestedClass Nested { get; set; }

        public virtual int OverrideInt { get; set; }
        public virtual object NewObject { get; set; }
    }
}
