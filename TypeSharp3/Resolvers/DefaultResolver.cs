using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        if (type.IsGenericType)
        {
            var typeName = type.Name[..type.Name.IndexOf('`')];

            if (type.IsGenericTypeDefinition)
            {
                general = new Lazy<IGeneralType>(() =>
                {
                    return new TypeReference(typeName);
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
            }
            else
            {
                _ = Parser.GetOrCreateGeneralType(type.GetGenericTypeDefinition());
                general = new Lazy<IGeneralType>(() =>
                {
                    return new TypeReference(typeName,
                    [
                        ..
                        from arg in type.GenericTypeArguments
                        select Parser.GetOrCreateGeneralType(arg)
                    ]);
                });
                declaration = null;
            }
            return true;
        }
        else
        {
            var typeName = type.Name;

            general = new Lazy<IGeneralType>(() => new TypeReference(typeName));
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
