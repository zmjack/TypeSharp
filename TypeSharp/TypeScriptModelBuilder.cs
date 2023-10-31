using NStandard;
using NStandard.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeSharp
{
    public class TypeScriptModelBuilder
    {
        public CacheSet<Type, TsType> TsTypes { get; private set; }
        public Dictionary<Type, string> DeclaredTypes { get; private set; } = new Dictionary<Type, string>();

        private readonly ModelBuilderOptions _options;
        public TypeScriptModelBuilder(ModelBuilderOptions options = null)
        {
            _options = options ??= ModelBuilderOptions.Default;

            TsTypes = new CacheSet<Type, TsType>
            {
                CacheMethodBuilder = type =>
                {
                    return () =>
                    {
                        if (DeclaredTypes.ContainsKey(type))
                        {
                            return new TsType(this, type, false) { TypeName = DeclaredTypes[type], Declare = true };
                        }

                        if (type.IsGenericType)
                        {
                            var typeDefinition = type.GetGenericTypeDefinition();
                            if (DeclaredTypes.ContainsKey(typeDefinition))
                            {
                                return new TsType(this, type, false) { TypeName = DeclaredTypes[typeDefinition], Declare = true };
                            }
                        }

                        if (_options.Verbose)
                        {
                            Console.WriteLine($"[Type] Cache: {type.FullName}");
                        }

                        // If type is generic, the full name is null.
                        if (type.FullName?.StartsWith("System.ValueTuple") ?? false)
                        {
                            var gtypes = type.GetGenericArguments();
                            var length = gtypes.Length;
                            //TODO: to supoort more items
                            if (length > 7) throw new NotSupportedException($"ValueType supports a maximum of 7 parameters.");

                            var sb = new StringBuilder();
                            sb.Append("{ ");
                            for (int i = 0; i < length; i++)
                            {
                                sb.Append($"Item{i + 1}: {TsTypes[gtypes[i]].Value.TypeName}");
                                if (i != length - 1) sb.Append(", ");
                            }
                            sb.Append(" }");

                            return new TsType(this, type, false) { TypeName = sb.ToString(), Declare = true };
                        }

                        return type switch
                        {
                            Type when type == typeof(bool) => new TsType(this, type, false) { TypeName = "boolean", Declare = true },
                            Type when type == typeof(string) => new TsType(this, type, false) { TypeName = "string", Declare = true },
                            Type when type == typeof(Guid) => new TsType(this, type, false) { TypeName = "string", Declare = true },
                            Type when type == typeof(byte) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(sbyte) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(char) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(short) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(ushort) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(int) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(uint) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(long) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(ulong) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(float) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(double) => new TsType(this, type, false) { TypeName = "number", Declare = true },
                            Type when type == typeof(decimal) => new TsType(this, type, false) { TypeName = "number", Declare = true },
#if NET6_0_OR_GREATER
                            Type when type == typeof(DateOnly) => new TsType(this, type, false) { TypeName = "Date", Declare = true },
#endif
                            Type when type == typeof(DateTime) => new TsType(this, type, false) { TypeName = "Date", Declare = true },
                            Type when type == typeof(DateTimeOffset) => new TsType(this, type, false) { TypeName = "Date", Declare = true },
                            Type when type == typeof(Array) => new TsType(this, type, false) { TypeName = "any[]", Declare = true },
                            Type when type == typeof(object) => new TsType(this, type, false) { TypeName = "any", Declare = true },
                            Type when type.IsArray => ParseType(typeof(IEnumerable<>).MakeGenericType(type.GetElementType())),
                            Type when type.IsType(typeof(IDictionary<,>)) || type.IsImplement(typeof(IDictionary<,>)) => Any.Create(() =>
                            {
                                var genericTypes = type.GetGenericArguments();
                                var keyType = TsTypes[genericTypes[0]];
                                var valueType = TsTypes[genericTypes[1]];
                                //TODO: Type name need to be simplified.
                                return new TsType(this, type, false) { TypeName = $"{{ [key: string]: {valueType.Value.ReferenceName} }}", Declare = true };
                            }),
                            Type when type.IsType(typeof(IEnumerable<>)) || type.IsImplement(typeof(IEnumerable<>)) => ParseType(typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments()[0])),
                            Type when type.IsType(typeof(Nullable<>)) => TsTypes[type.GenericTypeArguments[0]].Value,
                            Type when type.IsEnum => ParseEnum(type),
                            Type when type.IsClass || type.IsValueType || type.IsInterface => ParseType(type),
                            _ => throw new NotSupportedException($"{type.FullName} is not supported."),
                        };
                    };
                },
            };
        }
        public Dictionary<FieldInfo, TsConst> TsConsts { get; private set; } = new Dictionary<FieldInfo, TsConst>();

        public void WriteTo(string path, BuildOptions options = null) => File.WriteAllText(path, Compile(options));

        public string Compile(BuildOptions options = null)
        {
            var code = new StringBuilder();
            code.AppendLine(BuildOptions.VersionDeclaring);

            #region Compile Types
            for (int skipCount = 0; skipCount < TsTypes.Count; skipCount++)
            {
                var cache = TsTypes.Values.Skip(skipCount).First();
                cache.Value.TsProperties.Update();
            }

            // Struct & Struct? should map to the same type, so 'Distinct' is required.
            var typeGroups = TsTypes.Values.Select(x => x.Value).Distinct().Where(x => !x.Declare).GroupBy(x => x.Namespace);
            if (typeGroups.Any()) code.AppendLine();
            foreach (var typeGroup in typeGroups)
            {
                var tsNamespace = typeGroup.Key;
                code.AppendLine($"declare namespace {tsNamespace} {{");
                foreach (var tsType in typeGroup)
                {
                    switch (tsType.TypeClass)
                    {
                        case TsTypeClass.Interface:
                            code.AppendLine($"{" ".Repeat(4)}interface {tsType.TypeName} {{");
                            foreach (var tsProperty in tsType.TsProperties.Value)
                            {
                                var typeString = tsProperty.PropertyTypeDefinition ?? tsProperty.PropertyType.ReferenceName;
                                typeString = typeString.RegexReplace(new Regex($@"(?<![\w\d\._]){tsNamespace}\."), "");
                                code.AppendLine($"{" ".Repeat(8)}{tsProperty.Property}{(tsProperty.Required ? "" : "?")}: {typeString};");
                            }
                            code.AppendLine($"{" ".Repeat(4)}}}");

                            if (options?.OutputNames ?? false)
                            {
                                code.AppendLine($"{" ".Repeat(4)}export const enum {tsType.PureName}_names {{");
                                foreach (var tsProperty in tsType.TsProperties.Value)
                                {
                                    code.AppendLine($"{" ".Repeat(8)}{tsProperty.Property} = '{tsProperty.ClrName}',");
                                }
                                code.AppendLine($"{" ".Repeat(4)}}}");
                            }
                            break;

                        case TsTypeClass.Enum:
                            code.AppendLine($"{" ".Repeat(4)}export const enum {tsType.TypeName} {{");
                            foreach (var tsEnumValue in tsType.TsEnumValues)
                            {
                                code.AppendLine($"{" ".Repeat(8)}{tsEnumValue.Name} = {tsEnumValue.Value},");
                            }
                            code.AppendLine($"{" ".Repeat(4)}}}");
                            break;
                    }
                }
                code.AppendLine($"}}");
            }
            #endregion

            #region Compile Consts
            var tsConstOuterGroups = TsConsts.Values.ToArray().GroupBy(x => x.OuterNamespace);
            if (tsConstOuterGroups.Any()) code.AppendLine();
            foreach (var tsConstOuterGroup in tsConstOuterGroups)
            {
                var tsConstInnerGroups = tsConstOuterGroup.GroupBy(x => x.InnerNamespace);
                code.AppendLine($"namespace {tsConstOuterGroup.Key} {{");
                foreach (var tsConstInnerGroup in tsConstInnerGroups)
                {
                    code.AppendLine($"{" ".Repeat(4)}export namespace {tsConstInnerGroup.Key} {{");
                    foreach (var tsConst in tsConstInnerGroup)
                    {
                        code.AppendLine($"{" ".Repeat(8)}export const {tsConst.ConstName}: {tsConst.ConstType.TypeName} = {tsConst.ConstValue};");
                    }
                    code.AppendLine($"{" ".Repeat(4)}}}");
                }
                code.AppendLine($"}}");
            }
            #endregion

            return code.ToString();
        }

        private string GetTsNamespace(Type type)
        {
            var attr = type.GetCustomAttribute<TypeScriptModelAttribute>();
            if (attr?.Namespace is null)
            {
                var dType = type.DeclaringType;
                if (dType is null) return type.Namespace;
                else return $"{GetTsNamespace(dType)}.{dType.Name}";
            }
            else return attr.Namespace;
        }

        private void CacheConsts(Type type)
        {
            var consts = type.GetFields().Where(x => x.IsStatic && x.IsLiteral && x.IsPublic);
            foreach (var field in consts)
            {
                TsConsts[field] = new TsConst
                {
                    OuterNamespace = GetTsNamespace(type),
                    InnerNamespace = type.Name,
                    ConstName = field.Name,
                    ConstType = TsTypes[field.FieldType].Value,
                    ConstValue = field.GetValue(null).For(v => v is string ? $"'{v}'" : v.ToString()),
                };
            }
        }

        private TsType ParseEnum(Type type)
        {
            return new TsType(this, type, false)
            {
                Namespace = GetTsNamespace(type),
                TypeName = type.Name,
                TsEnumValues = Enum.GetNames(type).Select(name => new TsEnumValue
                {
                    Name = name,
                    Value = (int)Enum.Parse(type, name),
                }).ToArray(),
            };
        }

        private TsType ParseType(Type type)
        {
            var tsNamespace = GetTsNamespace(type);

            CacheConsts(type);
            if (type.IsGenericType)
            {
                var pureName = type.Name.ExtractFirst(new Regex(@"([^`]+)"));
                var genericTypes = type.IsGenericTypeDefinition
                    ? type.GetGenericArguments().Select(x => x.Name)
                    : type.GetGenericArguments().Select(x => x.IsGenericParameter ? x.Name : TsTypes[x].Value.ReferenceName);
                var generics = genericTypes.Join(", ");
                var typeName = $"{pureName}<{generics}>";

                if (tsNamespace == "System.Collections.Generic" && pureName == "IEnumerable")
                {
                    return new TsType(this, type, true)
                    {
                        TypeName = $"{generics}[]",
                        Declare = true,
                    };
                }

                if (type.IsGenericTypeDefinition)
                {
                    return new TsType(this, type, true)
                    {
                        Namespace = tsNamespace,
                        TypeName = typeName,
                    };
                }
                else
                {
                    _ = TsTypes[type.GetGenericTypeDefinition()];
                    return new TsType(this, type, true)
                    {
                        Namespace = tsNamespace,
                        TypeName = typeName,
                        Declare = true,
                    };
                }
            }
            else
            {
                return new TsType(this, type, true)
                {
                    Namespace = tsNamespace,
                    TypeName = type.Name,
                };
            }
        }

        public void CacheTypes(params Type[] types) => types.Each(CacheType);
        public void CacheType<TType>() => CacheType(typeof(TType));
        public void CacheType(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            _ = TsTypes[type];
        }

        public void AddDeclaredType(Type type, string typeName)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            DeclaredTypes.Add(type, typeName);
        }

    }
}