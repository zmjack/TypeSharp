namespace TypeSharp;

public class ParserOption
{
    public bool CamelCase { get; set; }
    public Resolver[] Resolvers { get; set; } = [];
}
