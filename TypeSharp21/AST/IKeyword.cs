namespace TypeSharp.AST;

public interface IKeyword<T>
{
    static abstract T Default { get; }
}
