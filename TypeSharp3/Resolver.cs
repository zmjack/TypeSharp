namespace TypeSharp;

public abstract class Resolver
{
    private TypeScriptGenerator? _parser;
    protected TypeScriptGenerator Parser
    {
        get
        {
            if (_parser is null) throw new InvalidOperationException("The resolver has not yet been bound.");
            return _parser;
        }
    }

    public Resolver() { }
    protected Resolver(TypeScriptGenerator parser)
    {
        _parser = parser;
    }

    internal void SetParser(TypeScriptGenerator parser)
    {
        _parser = parser;
    }

    public abstract bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general);
}
