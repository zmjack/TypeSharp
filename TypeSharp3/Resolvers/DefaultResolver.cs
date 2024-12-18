using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        if (type.IsGenericType)
        {
            if (type.IsGenericTypeDefinition)
            {
                general = new Lazy<IGeneralType>(() =>
                {
                    var name = type.Name[..type.Name.IndexOf('`')];
                    return new TypeReference(name);
                });
                declaration = new Lazy<IDeclaration>(() =>
                {
                    var name = type.Name[..type.Name.IndexOf('`')];
                    var @interface = new InterfaceDeclaration(name,
                    [
                        ..
                        from g in type.GetGenericArguments()
                        select new TypeParameter(g.Name)
                    ]);
                    var members = new List<InterfaceDeclaration.IMember>();

                    var props = type.GetProperties();
                    foreach (var prop in props)
                    {
                        var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                        members.Add(new PropertySignature(prop.Name, general));
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
                    var name = type.Name[..type.Name.IndexOf('`')];
                    return new TypeReference(name,
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
        else if (type.IsGenericParameter)
        {
            general = new Lazy<IGeneralType>(() => new TypeReference(type.Name));
            declaration = null;
            return true;
        }
        else
        {
            general = new Lazy<IGeneralType>(() => new TypeReference(type.Name));
            declaration = new Lazy<IDeclaration>(() =>
            {
                var @interface = new InterfaceDeclaration(type.Name);
                var members = new List<InterfaceDeclaration.IMember>();

                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                    members.Add(new PropertySignature(prop.Name, general));
                }
                @interface.Members = [.. members];

                return @interface;
            });
            return true;
        }
    }
}
