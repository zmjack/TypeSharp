using TypeSharp.AST;

namespace TypeSharp;

internal static class ClrTypeUtil
{
    public static IEnumerable<string> GetNamespaces(Type type)
    {
        var namespaces = type.Namespace?.Split('.') ?? [];
        foreach (var ns in namespaces)
        {
            yield return ns;
        }

        var stack = new Stack<string>();
        var current = type.DeclaringType;
        while (current is not null)
        {
            stack.Push(current.Name);
            current = current.DeclaringType;
        }

        foreach (var ns in stack)
        {
            yield return ns;
        }
    }

    public static string? GetNestedNamespace(IEnumerable<string> namespaces)
    {
        if (namespaces.Any())
        {
            return string.Join('.', namespaces);
        }
        else return null;
    }

    public static IIdentifier GetIdentifier(bool @useNamespace, Type type, string typeName)
    {
        if (@useNamespace)
        {
            var namespaces = GetNamespaces(type);
            if (namespaces.Any())
            {
                return new QualifiedName($"{GetNestedNamespace(namespaces)}.{typeName}");
            }
        }

        return new Identifier(typeName);
    }

    public static bool IsNullableValue(Type type)
    {
        if (type.IsInterface) return true;
        if (type.IsClass) return true;

        if (type.IsGenericType)
        {
            return type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        return false;
    }

}
