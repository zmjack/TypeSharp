namespace TypeSharp;

[Flags]
public enum IntegrationCodes
{
    None = 0,
    DeclareOnly = 0x01,
    SaveFile = 0x10,
}
