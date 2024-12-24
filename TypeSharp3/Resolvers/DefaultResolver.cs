using System.Collections;
using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    private static IGeneralType Wrap(Type type, TypeReference typeReference)
    {
        if (type.IsClass)
        {
            return new UnionType([typeReference, UndefinedKeyword.Default]);
        }
        else
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    return new UnionType([typeReference, UndefinedKeyword.Default]);
                }
            }
        }

        return typeReference;
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
                    return TypeReference.Array(Parser.GetOrCreateGeneralType(el));
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
                        return TypeReference.Array(AnyKeyword.Default);
                    });
                }
                else if (enumerable1 is not null)
                {
                    return new Lazy<IGeneralType>(() =>
                    {
                        var el = type.GetGenericArguments()[0];
                        return TypeReference.Array(Parser.GetOrCreateGeneralType(el));
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
                return Wrap(type, new TypeReference(referenceName));
            });
            declaration = new Lazy<IDeclaration>(() =>
            {
                var names = type.GetEnumNames();
                var values = type.GetEnumValuesAsUnderlyingType().OfType<object>().Select(Convert.ToInt64);
                var pairs = names.Zip(values);

                var enumDeclaration = new EnumDeclaration(typeName,
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
            var typeName = type.Name[..type.Name.IndexOf('`')];
            if (type.IsGenericTypeDefinition)
            {
                general = new Lazy<IGeneralType>(() =>
                {
                    var useNamespace = Parser.ModuleCode != ModuleCode.None;
                    var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                    return Wrap(type, new TypeReference(referenceName));
                });
                declaration = new Lazy<IDeclaration>(() =>
                {
                    var @interface = new InterfaceDeclaration(typeName,
                    [
                        ..
                        from g in type.GetGenericArguments()
                        select new TypeParameter(g.Name)
                    ]);
                    var members = new List<InterfaceDeclaration.IMember>();

                    var props = type.GetProperties();
                    foreach (var prop in props)
                    {
                        var propName = prop.Name;
                        if (Parser.CamelCase) propName = StringEx.CamelCase(propName);

                        var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                        members.Add(new PropertySignature(propName, general));
                    }
                    @interface.Members = [.. members];

                    return @interface;
                });
                return true;
            }
            else
            {
                _ = Parser.GetOrCreateGeneralType(type.GetGenericTypeDefinition());
                general = new Lazy<IGeneralType>(() =>
                {
                    var useNamespace = Parser.ModuleCode != ModuleCode.None;
                    var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                    return Wrap(type, new TypeReference(referenceName,
                    [
                        ..
                        from arg in type.GenericTypeArguments
                        select Parser.GetOrCreateGeneralType(arg)
                    ]));
                });
                declaration = null;
                return true;
            }
        }
        else
        {
            var typeName = type.Name;
            general = new Lazy<IGeneralType>(() =>
            {
                var useNamespace = Parser.ModuleCode != ModuleCode.None;
                var referenceName = ClrTypeUtil.GetIdentifier(useNamespace, type, typeName);
                return Wrap(type, new TypeReference(referenceName));
            });
            declaration = new Lazy<IDeclaration>(() =>
            {
                var @interface = new InterfaceDeclaration(typeName);
                var members = new List<InterfaceDeclaration.IMember>();

                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var propName = prop.Name;
                    if (Parser.CamelCase) propName = StringEx.CamelCase(propName);

                    var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                    members.Add(new PropertySignature(propName, general));
                }
                @interface.Members = [.. members];

                return @interface;
            });
            return true;
        }
    }
}
