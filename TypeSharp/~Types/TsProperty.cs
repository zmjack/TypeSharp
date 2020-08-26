namespace TypeSharp
{
    public class TsProperty
    {
        public string ClrName { get; set; }
        public string PropertyName { get; set; }
        public TsType PropertyType { get; set; }
        public string PropertyTypeDefinition { get; set; }
        public bool Required { get; set; }

    }
}
