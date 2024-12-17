using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    protected override bool TryResolve(Type type, out Lazy<IStatement>? statement)
    {
        statement = new Lazy<IStatement>(() =>
        {
            var declaration = new InterfaceDeclaration(type.Name);
            var members = new List<InterfaceDeclaration.IMember>();

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var general = Parser.GetOrCreateGeneralType(prop.PropertyType);
                members.Add(new PropertySignature(prop.Name, general));
            }
            declaration.Members = [.. members];

            return declaration;
        });

        return true;
    }
}
