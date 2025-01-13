namespace TypeSharp.Units;

/// <summary>
/// <see cref="ScriptClass"/> /
/// <see cref="ScriptEnum"/> /
/// <see cref="ScriptInterface"/> /
/// <see cref="ScriptUnionType"/>
/// </summary>
public class ScriptType : INameable
{
    public static readonly ScriptType Undefined = Builtin("undefined");
    public static readonly ScriptType Null = Builtin("null");
    public static readonly ScriptType Never = Builtin("never");
    public static readonly ScriptType Number = Builtin("number");
    public static readonly ScriptType String = Builtin("string");
    public static readonly ScriptType Boolean = Builtin("boolean");
    public static readonly ScriptType Date = Builtin("Date");
    public static readonly ScriptType Any = Builtin("any");
    public static readonly ScriptType Unknown = Builtin("unknown");

    public ScriptNamespace? Namespace { get; internal set; }
    public string Name { get; internal set; }
    public virtual QualifiedName FullName => new(Namespace?.Name, Name);

    public ScriptUnderlayingType TargetType { get; internal set; }
    public object Target { get; internal set; }
    public bool IsUnion { get; internal set; }
    public bool Array { get; internal set; }

    internal ScriptType()
    {
    }

    internal ScriptType(string name)
    {
        Name = name;
    }

    private static ScriptType Builtin(string name)
    {
        return new()
        {
            Name = name,
            TargetType = ScriptUnderlayingType.Builtin,
        };
    }

    public ScriptType MakeArrayType()
    {
        return new ScriptType($"{Name}[]", this)
        {
            Array = true,
        };
    }

    public ScriptType(string name, ScriptGeneric generic)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Generic;
        Target = generic;
    }

    public ScriptType(string name, ScriptType type)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Type;
        Target = type;
    }

    public ScriptType(string name, ScriptInterface @interface)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Interface;
        Target = @interface;
    }

    public ScriptType(string name, ScriptEnum @enum)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Enum;
        Target = @enum;
    }

    public ScriptType(string name, ScriptClass @class)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Class;
        Target = @class;
    }

    public ScriptType(string name, ScriptMethod method)
    {
        Name = name;
        TargetType = ScriptUnderlayingType.Method;
        Target = method;
    }
}
