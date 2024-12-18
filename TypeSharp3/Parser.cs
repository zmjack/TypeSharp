using System.Collections;
using TypeSharp.AST;
using TypeSharp.Resolvers;

namespace TypeSharp;

public class Parser : IEnumerable<INode>
{
    public bool CamelCase { get; private set; }
    public bool IncludeSaveCode { get; private set; }

    private readonly DefaultResolver _defaultResolver;

    public Parser()
    {
        CamelCase = false;
        IncludeSaveCode = false;
        _defaultResolver = new DefaultResolver();
        _defaultResolver.SetParser(this);
    }
    public Parser(ParserOption option)
    {
        CamelCase = option.CamelCase;
        IncludeSaveCode = option.GenerateSaveFileCode;
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

    private readonly Stack<Lazy<IDeclaration>> _heads = [];
    private readonly Dictionary<Type, Lazy<IGeneralType>> _generals = new()
    {
        [typeof(void)] = new(() => VoidKeyword.Default),
        [typeof(string)] = new(() => new UnionType([StringKeyword.Default, UndefinedKeyword.Default])),
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
        if (type.IsGenericParameter)
        {
            _generals.Add(type, new Lazy<IGeneralType>(() => new TypeReference(type.Name)));
        }
        else
        {
            IEnumerable<Resolver> resolvers = [.. _resolvers, _defaultResolver];
            foreach (var resolver in resolvers)
            {
                if (resolver.TryResolve(type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general))
                {
                    if (declaration is not null)
                    {
                        AddDeclaration(declaration);
                    }
                    if (general is not null)
                    {
                        _generals.Add(type, general);
                    }
                    return;
                }
            }

            throw new InvalidOperationException($"Can not add type. ({type})");
        }
    }

    public void Add(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Add(type);
        }
    }

    public IGeneralType GetOrCreateGeneralType(Type type)
    {
        if (!_generals.ContainsKey(type))
        {
            Add(type);
        }

        return _generals[type].Value;
    }

    public string GetCode()
    {
        var uncreatedItems = _heads.Where(x => !x.IsValueCreated).ToArray();
        while (uncreatedItems.Length > 0)
        {
            foreach (var item in uncreatedItems)
            {
                var value = item.Value;
            }
            uncreatedItems = _heads.Where(x => !x.IsValueCreated).ToArray();
        }

        var statements = new List<IStatement>();
        if (IncludeSaveCode)
        {
            statements.Add(new RawText(
                """
                function $ts_hcd(header: string) {
                  if (header == null) return defaultName;
                  var name = (regex: RegExp) => {
                    var match: RegExpExecArray;
                    if ((match = regex.exec(header)) !== null)
                      return decodeURI(match[1]);
                    else return null;
                  }
                  return name(/(?:filename\*=UTF-8'')([^;$]+)/g) ?? name(/(?:filename=)([^;$]+)/g);
                }
                function $ts_save(blob: Blob, filename: string) {
                  if (window.navigator['msSaveOrOpenBlob']) {
                    window.navigator['msSaveOrOpenBlob'](blob, filename);
                  } else {
                    var el = document.createElement('a');
                    var href = window.URL.createObjectURL(blob);
                    el.href = href;
                    el.download = filename;
                    document.body.appendChild(el);
                    el.click();
                    document.body.removeChild(el);
                    window.URL.revokeObjectURL(href);
                  }
                }
                """));
        }
        statements.AddRange(from inter in _heads select inter.Value);

        var source = new SourceFile
        {
            Statements = [.. statements]
        };
        return source.GetText();
    }

    public void AddDeclaration(Lazy<IDeclaration> declaration)
    {
        _heads.Push(declaration);
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
