namespace TypeSharp;

public enum ModuleCode
{
    None = 0,
    Nested = 1,

    [Obsolete("Not supported.", true)]
    Combined = 2,
}
