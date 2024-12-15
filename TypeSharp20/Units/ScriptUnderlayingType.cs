namespace TypeSharp.Units;

public enum ScriptUnderlayingType
{
    None = 0,

    Builtin = 0x1,
    Type = 0x2,

    Interface = 0x4,
    Enum = 0x8,

    Class = 0x10,
    Method = 0x20,

    Generic = 0x1000,
}
