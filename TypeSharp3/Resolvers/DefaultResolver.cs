using System.Collections;
using System.Reflection;
using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    private static IGeneralType Nullable(TypeReference typeReference)
    {
        return new UnionType([typeReference, UndefinedKeyword.Default]);
    }

    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        Lazy<IGeneralType>? AsArrayOrDefault()
        {
            if (type.IsArray)
            {
                return new Lazy<IGeneralType>(() =>
                {
                    var el = type.GetElementType()!;
                    var typeReference = TypeReference.Array(Parser.GetOrCreateGeneralType(el));
                    return Nullable(typeReference);
                });
            }
            else
            {
                Type[] interfaces = [type, .. type.GetInterfaces()];
                var enumerable1 = (
                    from x in interfaces
                    where x.IsGenericType
                    let e = x.IsGenericTypeDefinition ? x : x.GetGenericTypeDefinition()
                    where e == typeof(IEnumerable<>)
                    select x
                ).FirstOrDefault();

                if (interfaces.Any(x => x == typeof(IEnumerable)))
                {
                    return new Lazy<IGeneralType>(() =>
                    {
                        var typeReference = TypeReference.Array(AnyKeyword.Default);
                        return Nullable(typeReference);
                    });
                }
                else if (enumerable1 is not null)
                {
                    return new Lazy<IGeneralType>(() =>
                    {
                        var el = type.GetGenericArguments()[0];
                        var typeReference = TypeReference.Array(Parser.GetOrCreateGeneralType(el));
                        return Nullable(typeReference);
                    });
                }
                else return null;
            }
        }

        var arr = AsArrayOrDefault();
        if (arr is not null)
        {
            declaration = null;
            general = arr;
            return true;
        }

        if (type.IsEnum)
        {
            var typeName = type.Name;
            general = new Lazy<IGeneralType>(() =>
            {
                var useNamespace = Parser.ModuleCode != ModuleCode.None;
                var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                var typeReference = new TypeReference(referenceName);
                return type.IsClass ? Nullable(typeReference) : typeReference;
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
        else if (type.IsGenericType)
        {
            if (type.IsGenericTypeDefinition)
            {
                var typeName = type.Name[..type.Name.IndexOf('`')];
                general = new Lazy<IGeneralType>(() =>
                {
                    var useNamespace = Parser.ModuleCode != ModuleCode.None;
                    var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                    var typeReference = new TypeReference(referenceName,
                    [
                        ..
                        from arg in ((TypeInfo)type).GenericTypeParameters
                        select Parser.GetOrCreateGeneralType(arg)
                    ]);
                    return type.IsClass ? Nullable(typeReference) : typeReference;
                });
                declaration = new Lazy<IDeclaration>(() =>
                {
                    var @interface = new InterfaceDeclaration([ExportKeyword.Default], typeName,
                    [
                        ..
                        from g in type.GetGenericArguments()
                        select new TypeParameter(g.Name)
                    ]);
                    var members = new List<InterfaceDeclaration.IMember>();

                    var props = from p in type.GetProperties() group p by p.Name into g select g.First();
                    foreach (var prop in props)
                    {
                        if (prop.GetCustomAttribute<TypeScriptIgnoreAttribute>() is not null) continue;

                        var propName = prop.Name;
                        if (Parser.CamelCase) propName = StringEx.CamelCase(propName);

                        var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                        if (type.IsClass || ClrTypeUtil.IsNullableValue(type))
                        {
                            members.Add(new PropertySignature(propName, general)
                            {
                                QuestionToken = new(),
                            });
                        }
                        else
                        {
                            members.Add(new PropertySignature(propName, general));
                        }
                    }
                    @interface.Members = [.. members];

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
                        var typeReference = Parser.GetOrCreateGeneralType(nullableValueType);
                        return new UnionType([typeReference, UndefinedKeyword.Default]);
                    });
                    declaration = null;
                    return true;
                }
                else
                {
                    var typeName = type.Name[..type.Name.IndexOf('`')];
                    _ = Parser.GetOrCreateGeneralType(type.GetGenericTypeDefinition());
                    general = new Lazy<IGeneralType>(() =>
                    {
                        var useNamespace = Parser.ModuleCode != ModuleCode.None;
                        var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                        var typeReference = new TypeReference(referenceName,
                        [
                            ..
                            from arg in type.GenericTypeArguments
                            select Parser.GetOrCreateGeneralType(arg)
                        ]);
                        return type.IsClass ? Nullable(typeReference) : typeReference;
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
                var useNamespace = Parser.ModuleCode != ModuleCode.None;
                var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                var typeReference = new TypeReference(referenceName);
                return type.IsClass ? Nullable(typeReference) : typeReference;
            });
            declaration = new Lazy<IDeclaration>(() =>
            {
                var @interface = new InterfaceDeclaration([ExportKeyword.Default], typeName);
                var members = new List<InterfaceDeclaration.IMember>();

                var props = from p in type.GetProperties() group p by p.Name into g select g.First();
                foreach (var prop in props)
                {
                    if (prop.GetCustomAttribute<TypeScriptIgnoreAttribute>() is not null) continue;

                    var propName = prop.Name;
                    if (Parser.CamelCase) propName = StringEx.CamelCase(propName);

                    var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                    if (type.IsClass || ClrTypeUtil.IsNullableValue(type))
                    {
                        members.Add(new PropertySignature(propName, general)
                        {
                            QuestionToken = new(),
                        });
                    }
                    else
                    {
                        members.Add(new PropertySignature(propName, general));
                    }
                }
                @interface.Members = [.. members];

                return @interface;
            });
            return true;
        }
    }
}
