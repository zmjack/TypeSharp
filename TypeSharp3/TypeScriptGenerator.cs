using System.Collections;
using System.Reflection;
using TypeSharp.AST;
using TypeSharp.Resolvers;

namespace TypeSharp;

public class TypeScriptGenerator : IEnumerable<INode>
{
    public bool CamelCase { get; private set; }
    public string[] IntegrationCodes { get; private set; }
    public ModuleCode ModuleCode { get; private set; }
    public string? HeaderCode { get; set; }
    public string? FooterCode { get; set; }

    private readonly DefaultResolver _defaultResolver;

    public TypeScriptGenerator()
    {
        CamelCase = false;
        IntegrationCodes = [];
        ModuleCode = ModuleCode.None;

        _defaultResolver = new DefaultResolver();
        _defaultResolver.SetParser(this);
    }
    public TypeScriptGenerator(TypeScriptGeneratorOption option)
    {
        CamelCase = option.CamelCase;
        IntegrationCodes = option.IntegrationCodes ?? [];
        ModuleCode = option.ModuleCode;
        HeaderCode = option.HeaderCode;
        FooterCode = option.FooterCode;

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

        if (option.DetectionMode == DetectionMode.AutoDetect)
        {
            var assembly = Assembly.GetCallingAssembly()!;
            var types = assembly.GetTypes().Where(x => x.GetCustomAttribute<TypeScriptGeneratorAttribute>() is not null);
            foreach (var type in types)
            {
                Add(type);
            }
        }
    }

    private readonly Dictionary<Type, Lazy<IDeclaration>> _declarations = [];
    private readonly Dictionary<Type, Lazy<IGeneralType>> _generals = new()
    {
        [typeof(object)] = new(() => AnyKeyword.Default),
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

    [Obsolete("Need to be removed in the future.")]
    public void AddTypeResolver(Type type, Lazy<IGeneralType> resolver)
    {
        _generals.Add(type, resolver);
    }

    public void Add(Type type)
    {
        if (type.IsGenericParameter)
        {
            if (type.IsClass)
            {
                _generals.Add(type, new Lazy<IGeneralType>(() =>
                {
                    return new UnionType([
                        new TypeReference(new Identifier(type.Name)),
                        UndefinedKeyword.Default
                    ]);
                }));
            }
            else
            {
                _generals.Add(type, new Lazy<IGeneralType>(() =>
                {
                    return new TypeReference(new Identifier(type.Name));
                }));
            }
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
                        _declarations.Add(type, declaration);
                    }
                    if (general is not null)
                    {
                        _generals.Add(type, general);
                    }
                    return;
                }
            }

            throw new InvalidOperationException($"Can not resolve type. (Type: {type})");
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
        var uncreatedItems = _declarations.Where(x => !x.Value.IsValueCreated).ToArray();
        while (uncreatedItems.Length > 0)
        {
            foreach (var item in uncreatedItems)
            {
                var value = item.Value.Value;
            }
            uncreatedItems = _declarations.Where(x => !x.Value.IsValueCreated).ToArray();
        }

        var statements = new List<IStatement>
        {
            new RawText("/* Generated by TypeSharpV3 dev-version */")
        };

        if (HeaderCode is not null)
        {
            statements.Add(new RawText(HeaderCode));
        }

        if (IntegrationCodes.Length != 0)
        {
            var code = IntegrationCode.GetCode(IntegrationCodes);
            statements.Add(new RawText(code));
        }

        var comparer = new NamespaceComparer();
        if (ModuleCode == ModuleCode.None)
        {
            statements.AddRange((
                from declaration in _declarations
                let namespaces = ClrTypeUtil.GetNamespaces(declaration.Key).ToArray()
                select new
                {
                    Declaration = declaration,
                    Namespaces = namespaces,
                })
                .OrderBy(x => x.Namespaces, comparer)
                .ThenBy(x => x.Declaration.Key.Name)
                .Select(x => x.Declaration.Value.Value)
            );
        }
        else if (ModuleCode == ModuleCode.Nested)
        {
            var modules =
                from pair in (
                    from declaration in _declarations
                    let namespaces = ClrTypeUtil.GetNamespaces(declaration.Key).ToArray()
                    let nestedNamespace = ClrTypeUtil.GetNestedNamespace(namespaces)
                    select new
                    {
                        Declaration = declaration,
                        Namespaces = namespaces,
                        NestedNamespace = nestedNamespace,
                    })
                    .OrderBy(x => x.Namespaces, comparer)
                    .ThenByDescending(x => x.Declaration.Value.Value.Kind)
                    .ThenBy(x => x.Declaration.Key.Name)
                group pair by pair.NestedNamespace;

            foreach (var module in modules)
            {
                if (module.Key is not null)
                {
                    statements.Add(new ModuleDeclaration([ExportKeyword.Default], module.Key)
                    {
                        Body = new ModuleBlock()
                        {
                            Statements =
                            [
                                ..
                                from pair in module select pair.Declaration.Value.Value
                            ]
                        }
                    });
                }
                else
                {
                    statements.AddRange(from pair in module select pair.Declaration.Value.Value);
                }
            }
        }

        if (FooterCode is not null)
        {
            statements.Add(new RawText(FooterCode));
        }

        var source = new SourceFile
        {
            Statements = [.. statements]
        };
        return source.GetText();
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
