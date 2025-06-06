using System.Collections;
using System.Reflection;
using TypeSharp.AST;

namespace TypeSharp;

public class ResolverUtil
{
    private readonly TypeScriptGenerator _generator;
    private static readonly string _required = "System.Runtime.CompilerServices.RequiredMemberAttribute";

    internal ResolverUtil(TypeScriptGenerator generator)
    {
        _generator = generator;
    }

    private PropertySignature GetPropertySignature(PropertyInfo property)
    {
        var propertyName = property.Name;
        if (_generator.CamelCase)
        {
            propertyName = StringEx.CamelCase(propertyName);
        }
        var propertyType = property.PropertyType;

        var general = _generator.GetOrCreateGeneralType(propertyType);
        var required = property.GetCustomAttributes().Any(x => x.GetType().FullName == _required);
        if (required)
        {
            if (propertyType == typeof(string))
            {
                return new(propertyName, StringKeyword.Default);
            }
            if (propertyType.IsClass)
            {
                return new(propertyName, new UnionType([general, UndefinedKeyword.Default]));
            }
        }
        else
        {
            if (propertyType == typeof(string))
            {
                return new(propertyName, StringKeyword.Default)
                {
                    QuestionToken = QuestionToken.Default,
                };
            }
            if (ClrTypeUtil.IsNullable(propertyType))
            {
                return new(propertyName, general)
                {
                    QuestionToken = QuestionToken.Default,
                };
            }
        }
        return new(propertyName, general);
    }

    private string GetTypeName_Normal(Type type)
    {
        return type.Name;
    }
    private string GetTypeName_Generic(Type type)
    {
        return type.Name[..type.Name.IndexOf('`')];
    }

    public bool TryResolve_Dictionary(Type type, Type[] selfAndInterfaces, out Lazy<IGeneralType>? general, out Lazy<IDeclaration>? declaration)
    {
        var dictionary2 = (
            from x in selfAndInterfaces
            where x.IsGenericType
            let e = x.IsGenericTypeDefinition ? x : x.GetGenericTypeDefinition()
            where e == typeof(IDictionary<,>)
            select x
        ).FirstOrDefault();

        if (dictionary2 is not null)
        {
            general = new Lazy<IGeneralType>(() =>
            {
                var args = type.GetGenericArguments();
                var keyType = _generator.GetOrCreateGeneralType(args[0]);
                var valueType = _generator.GetOrCreateGeneralType(args[1]);

                //TODO: If the type is a number, then the type of IndexSignature is a number.
                //var typeLiteral = new TypeLiteral()
                //{
                //    Members =
                //    [
                //        new IndexSignature(new("key", StringKeyword.Default), valueType)
                //    ]
                //};
                var typeReference = TypeReference.Record(StringKeyword.Default, valueType);
                return typeReference;
            });
            declaration = null;
            return true;
        }
        else
        {
            general = null;
            declaration = null;
            return false;
        }
    }

    public bool TryResolve_Array(Type type, Type[] selfAndInterfaces, out Lazy<IGeneralType>? general, out Lazy<IDeclaration>? declaration)
    {
        if (type.IsArray)
        {
            general = new Lazy<IGeneralType>(() =>
            {
                var el = type.GetElementType()!;
                var typeReference = TypeReference.Array(_generator.GetOrCreateGeneralType(el));
                return typeReference;
            });
            declaration = null;
            return true;
        }
        else
        {
            var enumerable1 = (
                from x in selfAndInterfaces
                where x.IsGenericType
                let e = x.IsGenericTypeDefinition ? x : x.GetGenericTypeDefinition()
                where e == typeof(IEnumerable<>)
                select x
            ).FirstOrDefault();

            if (selfAndInterfaces.Any(x => x == typeof(IEnumerable)))
            {
                general = new Lazy<IGeneralType>(() =>
                {
                    var typeReference = TypeReference.Array(AnyKeyword.Default);
                    return typeReference;
                });
                declaration = null;
                return true;
            }
            else if (enumerable1 is not null)
            {
                general = new Lazy<IGeneralType>(() =>
                {
                    var el = type.GetGenericArguments()[0];
                    var typeReference = TypeReference.Array(_generator.GetOrCreateGeneralType(el));
                    return typeReference;
                });
                declaration = null;
                return true;
            }
            else
            {
                general = null;
                declaration = null;
                return false;
            }
        }
    }

    public bool TryResolve_Enum(Type type, out Lazy<IGeneralType>? general, out Lazy<IDeclaration>? declaration)
    {
        if (type.IsEnum)
        {
            var typeName = GetTypeName_Normal(type);
            general = new Lazy<IGeneralType>(() =>
            {
                var useNamespace = _generator.ModuleCode != ModuleCode.None;
                var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                var typeReference = new TypeReference(referenceName);
                return typeReference;
            });
            declaration = new Lazy<IDeclaration>(() =>
            {
                var names = type.GetEnumNames();
                var values = type.GetEnumValuesAsUnderlyingType().OfType<object>().Select(Convert.ToInt64);
                var pairs = names.Zip(values);

                var enumDeclaration = new EnumDeclaration([ExportKeyword.Default], typeName,
                [
                    ..
                    from item in pairs
                    select new EnumMember(item.First, new NumericLiteral(item.Second))
                ]);
                return enumDeclaration;
            });
            return true;
        }
        else
        {
            general = null;
            declaration = null;
            return false;
        }
    }

    public bool TryResolve_Normal(Type type, Type[] interfaces, out Lazy<IGeneralType>? general, out Lazy<IDeclaration>? declaration)
    {
        TypeReference[] extends =
        [
            ..
            from itype in interfaces
            let attr = itype.GetCustomAttribute<TypeScriptGeneratorAttribute>()
            where attr is not null
            let igeneral = _generator.GetOrCreateGeneralType(itype)
            where igeneral is TypeReference
            select igeneral as TypeReference
        ];

        var heritageClauses = new List<HeritageClause>();
        if (extends.Length != 0)
        {
            heritageClauses.Add(new HeritageClause(ExtendsKeyword.Default,
            [
                ..
                from e in extends
                select new ExpressionWithTypeArguments(e.TypeName, e.TypeArguments)
            ]));
        }

        if (type.IsGenericType)
        {
            if (type.IsGenericTypeDefinition)
            {
                var typeName = GetTypeName_Generic(type);
                general = new Lazy<IGeneralType>(() =>
                {
                    var useNamespace = _generator.ModuleCode != ModuleCode.None;
                    var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                    var typeReference = new TypeReference(referenceName,
                    [
                        ..
                        from arg in ((TypeInfo)type).GenericTypeParameters
                        select _generator.GetOrCreateGeneralType(arg)
                    ]);
                    return typeReference;
                });
                declaration = new Lazy<IDeclaration>(() =>
                {
                    var members = new List<InterfaceDeclaration.IMember>();
                    var props =
                        from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        group p by p.Name into g
                        select g.First();
                    foreach (var prop in props)
                    {
                        if (prop.GetCustomAttribute<TypeScriptIgnoreAttribute>() is not null) continue;
                        members.Add(GetPropertySignature(prop));
                    }

                    var @interface = new InterfaceDeclaration([ExportKeyword.Default], typeName,
                    [
                        ..
                        from g in type.GetGenericArguments()
                        select new TypeParameter(g.Name)
                    ])
                    {
                        Members = [.. members],
                        HeritageClauses = [.. heritageClauses],
                    };
                    return @interface;
                });
                return true;
            }
            else
            {
                if (ClrTypeUtil.IsNullableValue(type))
                {
                    var nullableValueType = type.GetGenericArguments()[0];
                    general = new Lazy<IGeneralType>(() =>
                    {
                        var typeReference = _generator.GetOrCreateGeneralType(nullableValueType);
                        return typeReference;
                    });
                    declaration = null;
                    return true;
                }
                else
                {
                    var typeName = GetTypeName_Generic(type);
                    _ = _generator.GetOrCreateGeneralType(type.GetGenericTypeDefinition());
                    general = new Lazy<IGeneralType>(() =>
                    {
                        var useNamespace = _generator.ModuleCode != ModuleCode.None;
                        var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                        var typeReference = new TypeReference(referenceName,
                        [
                            ..
                            from arg in type.GenericTypeArguments
                            select _generator.GetOrCreateGeneralType(arg)
                        ]);
                        return typeReference;
                    });
                    declaration = null;
                    return true;
                }
            }
        }
        else
        {
            var typeName = type.Name;
            general = new Lazy<IGeneralType>(() =>
            {
                var useNamespace = _generator.ModuleCode != ModuleCode.None;
                var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                var typeReference = new TypeReference(referenceName);
                return typeReference;
            });
            declaration = new Lazy<IDeclaration>(() =>
            {
                var members = new List<InterfaceDeclaration.IMember>();
                var props =
                    from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    group p by p.Name into g
                    select g.First();
                foreach (var prop in props)
                {
                    if (prop.GetCustomAttribute<TypeScriptIgnoreAttribute>() is not null) continue;
                    members.Add(GetPropertySignature(prop));
                }

                var @interface = new InterfaceDeclaration([ExportKeyword.Default], typeName)
                {
                    Members = [.. members],
                    HeritageClauses = [.. heritageClauses],
                };
                return @interface;
            });
            return true;
        }
    }

}
