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

    protected abstract bool TryResolve(Type type, out Lazy<IStatement>? statement);
    public bool TryRun(Type type, out Lazy<IGeneralType>? output)
    {
        var parser = Parser;
        if (TryResolve(type, out var statement))
        {
            Parser.AddHead(type, statement!);
            output = new(() => new TypeReference(type.Name));
            return true;
        }

        output = null;
        return false;
    }
}
