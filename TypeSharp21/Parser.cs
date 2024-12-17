using System.Collections;
using TypeSharp.AST;
using TypeSharp.Resolvers;

namespace TypeSharp;

public class Parser : IEnumerable<INode>
{
    private readonly bool _camelCase;
    private readonly DefaultResolver _defaultResolver;

    public Parser()
    {
        _camelCase = false;
        _defaultResolver = new DefaultResolver();
        _defaultResolver.SetParser(this);
    }
    public Parser(ParserOption option)
    {
        _camelCase = option.CamelCase;
        _defaultResolver = new DefaultResolver();
        _defaultResolver.SetParser(this);

        if (option.Resolvers is not null)
        {
            foreach (var resolver in option.Resolvers)
            {
                resolver.SetParser(this);
                _resolvers.Add(resolver);
            }
        }
    }

    private readonly Dictionary<Type, Lazy<IStatement>> _heads = [];
    private readonly Dictionary<Type, Lazy<IGeneralType>> _generalTypes = new()
    {
        [typeof(void)] = new(() => VoidKeyword.Default),
        [typeof(string)] = new(() => StringKeyword.Default),
        [typeof(bool)] = new(() => BooleanKeyword.Default),
        [typeof(Guid)] = new(() => StringKeyword.Default),
        [typeof(byte)] = new(() => NumberKeyword.Default),
        [typeof(sbyte)] = new(() => NumberKeyword.Default),
        [typeof(char)] = new(() => NumberKeyword.Default),
        [typeof(short)] = new(() => NumberKeyword.Default),
        [typeof(ushort)] = new(() => NumberKeyword.Default),
        [typeof(int)] = new(() => NumberKeyword.Default),
        [typeof(uint)] = new(() => NumberKeyword.Default),
        [typeof(long)] = new(() => NumberKeyword.Default),
        [typeof(ulong)] = new(() => NumberKeyword.Default),
        [typeof(float)] = new(() => NumberKeyword.Default),
        [typeof(double)] = new(() => NumberKeyword.Default),
        [typeof(decimal)] = new(() => NumberKeyword.Default),
#if NET6_0_OR_GREATER
        [typeof(DateOnly)] = new(() => TypeReference.Date),
        [typeof(DateTime)] = new(() => TypeReference.Date),
        [typeof(DateTimeOffset)] = new(() => TypeReference.Date),
#endif
    };
    private readonly List<Resolver> _resolvers = [];

    public void Add(Type type)
    {
        if (type.IsGenericType)
        {
            if (type.IsGenericTypeDefinition)
            {
                _generalTypes.Add(type, FromGenericType(type));
            }
            else
            {
                _generalTypes.Add(type, FromGenericType(type.GetGenericTypeDefinition()));
            }
        }
        else
        {
            var general = FromNormalType(type);
            if (general is not null)
            {
                _generalTypes.Add(type, general);
            }
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
        var uncreatedItems = _heads.Values.Where(x => !x.IsValueCreated).ToArray();
        while (uncreatedItems.Length > 0)
        {
            foreach (var item in uncreatedItems)
            {
                var value = item.Value;
            }
            uncreatedItems = _heads.Values.Where(x => !x.IsValueCreated).ToArray();
        }

        var source = new SourceFile
        {
            Statements =
            [
                ..
                from inter in _heads select inter.Value.Value,
            ]
        };
        return source.GetText();
    }

    public IGeneralType GetOrCreateGeneralType(Type type)
    {
        if (!_generalTypes.ContainsKey(type))
        {
            Add(type);
        }

        return _generalTypes[type].Value;
    }

    public void AddHead(Type type, Lazy<IStatement> lazy)
    {
        _heads.Add(type, lazy);
    }

    public Lazy<IGeneralType>? FromNormalType(Type type)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.TryRun(type, out var general))
            {
                return general!;
            }
        }

        if (_defaultResolver.TryRun(type, out var defaultGeneral))
        {
            return defaultGeneral;
        }

        return null;
    }

    public Lazy<IGeneralType> FromGenericType(Type type)
    {
        throw new NotImplementedException($"{type}");
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
