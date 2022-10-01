namespace TypeSharp.Test.Models
{
    [TypeScriptModel(Namespace = "TSNS3")]
    public class GenericClass<T>
    {
        public T Value { get; set; }
        public T[] Values { get; set; }
        public T[][] Values2 { get; set; }
        public GenericClass<int> IntValue { get; set; }
    }
}
