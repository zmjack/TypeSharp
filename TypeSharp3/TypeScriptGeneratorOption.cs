namespace TypeSharp;

public class TypeScriptGeneratorOption
{
    public bool CamelCase { get; set; }
    public ModuleCode ModuleCode { get; set; }
    public DetectionMode DetectionMode { get; set; }
    /// <summary>
    /// Use <see cref="IntegrationCode"/> values;
    /// </summary>
    public string[]? IntegrationCodes { get; set; }
    public Resolver[] Resolvers { get; set; } = [];
    public string? HeaderCode { get; set; }
    public string? FooterCode { get; set; }
}
