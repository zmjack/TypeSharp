namespace TypeSharp;

public interface INameable
{
    string Name { get; }
    QualifiedName FullName { get; }
}
