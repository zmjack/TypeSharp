using TypeSharp.Infrastructures;

namespace TypeSharp;

public interface IGenericGenerable
{
    ScriptType[] GenericArguments { get; set; }
}
