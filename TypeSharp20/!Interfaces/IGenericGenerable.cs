using TypeSharp.Units;

namespace TypeSharp;

public interface IGenericGenerable
{
    ScriptType[] GenericArguments { get; set; }
}
