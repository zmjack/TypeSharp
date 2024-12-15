using System.Collections;
using TypeSharp21.AST;

namespace TypeSharp21;

public class TypeScriptParser : IEnumerable<INode>
{
    public Dictionary<Type, Lazy<InterfaceDeclaration>> _interfaces = new()
    {
    };
    public Dictionary<Type, Lazy<IGeneralType>> _transforms = new()
    {
        [typeof(string)] = new(StringKeyword.Default),
        [typeof(bool)] = new(BooleanKeyword.Default),
        [typeof(Guid)] = new(StringKeyword.Default),
        [typeof(byte)] = new(NumberKeyword.Default),
        [typeof(sbyte)] = new(NumberKeyword.Default),
        [typeof(char)] = new(NumberKeyword.Default),
        [typeof(short)] = new(NumberKeyword.Default),
        [typeof(ushort)] = new(NumberKeyword.Default),
        [typeof(int)] = new(NumberKeyword.Default),
        [typeof(uint)] = new(NumberKeyword.Default),
        [typeof(long)] = new(NumberKeyword.Default),
        [typeof(ulong)] = new(NumberKeyword.Default),
        [typeof(float)] = new(NumberKeyword.Default),
        [typeof(double)] = new(NumberKeyword.Default),
        [typeof(decimal)] = new(NumberKeyword.Default),
#if NET6_0_OR_GREATER
        [typeof(DateOnly)] = new(TypeReference.Date),
        [typeof(DateTime)] = new(TypeReference.Date),
        [typeof(DateTimeOffset)] = new(TypeReference.Date),
#endif
    };

    public void Add(Type type)
    {
        if (type.IsGenericType)
        {
            if (type.IsGenericTypeDefinition)
            {
                _transforms.Add(type, FromGenericType(type));
            }
            else
            {
                _transforms.Add(type, FromGenericType(type.GetGenericTypeDefinition()));
            }
        }
        else
        {
            _transforms.Add(type, FromNormalType(type));
        }
    }

    public void Add(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Add(type);
        }
    }

    public string GetCode()
    {
        var uncreatedItems = _interfaces.Values.Where(x => !x.IsValueCreated).ToArray();
        while (uncreatedItems.Length > 0)
        {
            foreach (var item in uncreatedItems)
            {
                var value = item.Value;
            }
            uncreatedItems = _interfaces.Values.Where(x => !x.IsValueCreated).ToArray();
        }

        var source = new SourceFile
        {
            Statements =
            [
                ..
                from inter in _interfaces select inter.Value.Value,
            ]
        };
        return source.GetText();
    }

    private string[] GetNamespaces(Type type)
    {
        var namespaces = type.Namespace?.Split('.') ?? [];
        throw new NotImplementedException();
    }

    private IGeneralType GetGeneralType(Type type)
    {
        if (type == typeof(string)) return StringKeyword.Default;
        else if (type == typeof(bool)) return BooleanKeyword.Default;
        else if (type == typeof(Guid)) return StringKeyword.Default;
        else if (type == typeof(byte)) return NumberKeyword.Default;
        else if (type == typeof(sbyte)) return NumberKeyword.Default;
        else if (type == typeof(char)) return NumberKeyword.Default;
        else if (type == typeof(short)) return NumberKeyword.Default;
        else if (type == typeof(ushort)) return NumberKeyword.Default;
        else if (type == typeof(int)) return NumberKeyword.Default;
        else if (type == typeof(uint)) return NumberKeyword.Default;
        else if (type == typeof(long)) return NumberKeyword.Default;
        else if (type == typeof(ulong)) return NumberKeyword.Default;
        else if (type == typeof(float)) return NumberKeyword.Default;
        else if (type == typeof(double)) return NumberKeyword.Default;
        else if (type == typeof(decimal)) return NumberKeyword.Default;
#if NET6_0_OR_GREATER
        else if (type == typeof(DateOnly)) return TypeReference.Date;
        else if (type == typeof(DateTime)) return TypeReference.Date;
        else if (type == typeof(DateTimeOffset)) return TypeReference.Date;
#endif
        else return _transforms[type].Value;
    }

    public Lazy<IGeneralType> FromNormalType(Type type)
    {
        _interfaces.Add(type, new Lazy<InterfaceDeclaration>(() =>
        {
            var declaration = new InterfaceDeclaration(type.Name);
            var members = new List<InterfaceDeclaration.IMember>();

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!_transforms.ContainsKey(prop.PropertyType))
                {
                    Add(prop.PropertyType);
                }

                members.Add(new PropertySignature(prop.Name, GetGeneralType(prop.PropertyType)));
            }
            declaration.Members = [.. members];

            return declaration;
        }));

        return new(new TypeReference(type.Name));
    }

    public Lazy<IGeneralType> FromGenericType(Type type)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<INode> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
