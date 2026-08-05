using TypeSharp.AST;

namespace TypeSharp;

public abstract class Resolver
{
    private TypeScriptGenerator? _generator;
    protected TypeScriptGenerator Generator
    {
        get
        {
            if (_generator is null) throw new InvalidOperationException("The resolver has not yet been bound.");
            return _generator;
        }
    }

    public Resolver() { }
    protected Resolver(TypeScriptGenerator parser)
    {
        _generator = parser;
    }

    internal void SetParser(TypeScriptGenerator parser)
    {
        _generator = parser;
    }

    public abstract bool TryResolve(Type type, out Lazy<IGeneralType>? general, out Lazy<IDeclaration>? declaration);
}
