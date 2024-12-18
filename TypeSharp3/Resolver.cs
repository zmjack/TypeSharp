using TypeSharp.AST;

namespace TypeSharp;

public abstract class Resolver
{
    private Parser? _parser;
    protected Parser Parser
    {
        get
        {
            if (_parser is null) throw new InvalidOperationException("The resolver has not yet been bound.");
            return _parser;
        }
    }

    public Resolver() { }
    protected Resolver(Parser parser)
    {
        _parser = parser;
    }

    internal void SetParser(Parser parser)
    {
        _parser = parser;
    }

    public abstract bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general);
}
