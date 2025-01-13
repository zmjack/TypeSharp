using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TypeSharp.Attributes;
using TypeSharp.Units;

namespace TypeSharp;

public partial class TypeScriptWriter
{
    private const string IgnoreAttribute = "System.Text.Json.Serialization.JsonIgnoreAttribute";

    private readonly Indent _indent;

    public List<ScriptNamespace> Namespaces = [];
    public Dictionary<Type, ScriptClass> Classes = [];
    public Dictionary<Type, ScriptEnum> Enums = [];
    public Dictionary<Type, ScriptFunction> Functions = [];
    public Dictionary<Type, ScriptType> Types = [];

    public TypeScriptWriter(Indent indent)
    {
        _indent = indent;
    }

    private void AddNamespaces(params ScriptNamespace[] namespaces)
    {
        Namespaces.AddRange(namespaces);
    }

    private ScriptClass GetOrCreateClass(Type clrType, Func<ScriptClass> factory)
    {
        if (!Classes.ContainsKey(clrType))
        {
            Classes.Add(clrType, factory());
        }
        return Classes[clrType];
    }

    private ScriptEnum GetOrCreateEnum(Type clrType, Func<ScriptEnum> factory)
    {
        if (!Enums.ContainsKey(clrType))
        {
            Enums.Add(clrType, factory());
        }
        return Enums[clrType];
    }

    private ScriptFunction GetOrCreateFunction(Type clrType, Func<ScriptFunction> factory)
    {
        if (!Functions.ContainsKey(clrType))
        {
            Functions.Add(clrType, factory());
        }
        return Functions[clrType];
    }

    private ScriptType GetOrCreateType(Type clrType, Func<ScriptType> factory)
    {
        if (!Types.ContainsKey(clrType))
        {
            Types.Add(clrType, factory());
        }
        return Types[clrType];
    }

    public void AddClrType(Type clrType)
    {
        FromClrType(clrType);
    }


    [GeneratedRegex(@"([^`]+)")]
    private static partial Regex GetObjectNameRegex();
    private readonly Regex _objectNameRegex = GetObjectNameRegex();

    private static bool IsRequired(PropertyInfo property)
    {
        var attributes = property.GetCustomAttributes(true);
        var required = (
            from x in attributes
            let attributeType = x.GetType()
            where attributeType.FullName == "System.ComponentModel.DataAnnotations.RequiredAttribute"
                || attributeType == typeof(TypeScriptRequiredAttribute)
            select x
        ).Any();
        return required;
    }

    private ScriptParameter[] GetProperties(Type clrType)
    {
        var chain = clrType.GetExtendChain();
        var props = clrType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => !x.GetCustomAttributes().Any(attr => attr.GetType().FullName == IgnoreAttribute))
            .GroupBy(x => x.Name)
            .Select(g =>
            {
                if (g.Skip(1).Any()) return g.OrderBy(x => chain.IndexOf(x.DeclaringType)).First();
                else return g.First();
            }).ToArray();

        var list = new List<ScriptParameter>();
        foreach (var prop in props)
        {
            var propType = prop.PropertyType;
            var required = IsRequired(prop);

            if (propType.IsGenericParameter)
            {
                throw new NotImplementedException();
            }
            else
            {
                list.Add(new(StringEx.CamelCase(prop.Name), FromClrType(clrType)));
            }
        }

        return list.ToArray();
    }

    private ScriptType FromClrType(Type clrType)
    {
        //TODO: cache constants
        //CacheConsts(type);
        if (clrType.IsGenericType)
        {
            if (!clrType.IsGenericTypeDefinition)
            {
                var definition = clrType.GetGenericTypeDefinition();
                return FromClrType(definition);
            }

            var generics = clrType.GetGenericArguments().Select(x => new ScriptGeneric(x.Name)).ToArray();
            var name = clrType.Name.ExtractFirst(_objectNameRegex);
            var typeName = $"{name}<{generics}>";

            var isEnumerable = clrType.Namespace == "System.Collections.Generic" && name == "IEnumerable";
            if (isEnumerable)
            {
                return GetOrCreateType(clrType, () =>
                {
                    var element = generics[0];
                    var scriptType = new ScriptType(element.Name, element);
                    var arrayType = scriptType.MakeArrayType();
                    return arrayType;
                });
            }
            else
            {
                return GetOrCreateType(clrType, () =>
                {
                    var scriptType = new ScriptInterface(clrType.Name, generics)
                    {
                        Namespace = new(clrType.Namespace!)
                    };
                    return scriptType;
                });
            }
        }
        else
        {
            ScriptType scriptType;

            if (clrType == typeof(string)) scriptType = ScriptType.String;
            else if (clrType == typeof(bool)) scriptType = ScriptType.Boolean;
            else if (clrType == typeof(Guid)) scriptType = ScriptType.String;
            else if (clrType == typeof(byte)) scriptType = ScriptType.Number;
            else if (clrType == typeof(sbyte)) scriptType = ScriptType.Number;
            else if (clrType == typeof(char)) scriptType = ScriptType.Number;
            else if (clrType == typeof(short)) scriptType = ScriptType.Number;
            else if (clrType == typeof(ushort)) scriptType = ScriptType.Number;
            else if (clrType == typeof(int)) scriptType = ScriptType.Number;
            else if (clrType == typeof(uint)) scriptType = ScriptType.Number;
            else if (clrType == typeof(long)) scriptType = ScriptType.Number;
            else if (clrType == typeof(ulong)) scriptType = ScriptType.Number;
            else if (clrType == typeof(float)) scriptType = ScriptType.Number;
            else if (clrType == typeof(double)) scriptType = ScriptType.Number;
            else if (clrType == typeof(decimal)) scriptType = ScriptType.Number;
#if NET6_0_OR_GREATER
            else if (clrType == typeof(DateOnly)) scriptType = ScriptType.Date;
            else if (clrType == typeof(DateTime)) scriptType = ScriptType.Date;
            else if (clrType == typeof(DateTimeOffset)) scriptType = ScriptType.Date;
#endif
            else
            {
                var props = GetProperties(clrType);
                var @interface = new ScriptInterface(clrType.Name)
                {
                    Namespace = new(clrType.Namespace!)
                };

                foreach (var prop in props)
                {
                    @interface.Add(new ScriptInterface.Field(Access.Public, prop.Name, prop.Type));
                }
                scriptType = @interface;
            }

            return GetOrCreateType(clrType, () => scriptType);
        }
    }

    public string GetCode()
    {
        var sb = new StringBuilder();

        foreach (var (clrType, type) in Types)
        {
            if (type is ScriptInterface @interface)
            {
                sb.AppendLine(@interface.Encode());
            }
            else
            {
                sb.AppendLine(type.Name);
            }
        }

        return sb.ToString();
    }

}
