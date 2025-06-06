using TypeSharp.AST;

namespace TypeSharp.Resolvers;

public class DefaultResolver : Resolver
{
    public override bool TryResolve(Type type, out Lazy<IDeclaration>? declaration, out Lazy<IGeneralType>? general)
    {
        var util = new ResolverUtil(Generator);

        Type[] interfaces = type.GetInterfaces();
        Type[] selfAndInterfaces = [type, .. interfaces];

        if (util.TryResolve_Dictionary(type, selfAndInterfaces, out general, out declaration))
        {
            return true;
        }
        if (util.TryResolve_Array(type, selfAndInterfaces, out general, out declaration))
        {
            return true;
        }
        if (util.TryResolve_Enum(type, out general, out declaration))
        {
            return true;
        }
        if (util.TryResolve_Normal(type, interfaces, out general, out declaration))
        {
            return true;
        }

        return false;
    }
}
