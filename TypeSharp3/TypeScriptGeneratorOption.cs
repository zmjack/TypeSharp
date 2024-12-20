namespace TypeSharp;

public class TypeScriptGeneratorOption
{
    public bool CamelCase { get; set; }
    public ModuleCode ModuleCode { get; set; }
    public DetectionMode DetectionMode { get; set; }
    public IntegrationCodes IntegrationCodes { get; set; }
    public Resolver[] Resolvers { get; set; } = [];
}
