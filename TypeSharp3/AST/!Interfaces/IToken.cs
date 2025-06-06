namespace TypeSharp.AST;

public interface IToken<T>
{
    static abstract T Default { get; }
}
