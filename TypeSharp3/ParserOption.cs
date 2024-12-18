namespace TypeSharp;

public class ParserOption
{
    public bool CamelCase { get; set; }
    public bool GenerateSaveFileCode { get; set; }
    public Resolver[] Resolvers { get; set; } = [];
}
