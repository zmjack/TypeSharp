namespace TypeSharp.Test
{
    [TypeScriptModel]
    public class SimpleModel
    {
        public int Value { get; set; }

        [TypeScriptIgnore]
        public int Hidden { get; set; }
    }

}
