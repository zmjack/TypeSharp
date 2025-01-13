namespace TypeSharp
{
    public class TypeScriptPossibleValuesAttribute
    {
        public string[] PossibleValues { get; set; }

        public TypeScriptPossibleValuesAttribute(string[] possiblesValues)
        {
            PossibleValues = possiblesValues;
        }

    }
}
