﻿using NStandard;
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

        public TypeScriptModelBuilder()
        {
            TsTypes = new CacheSet<Type, TsType>
            {
                CacheMethodBuilder = type =>
                {
                    return () =>
                    {
                        if (DeclaredTypes.ContainsKey(type)) return new TsType(TsTypes, type, false) { TypeName = DeclaredTypes[type], Declare = true };

                        switch (type)
                        {
                            case Type _ when type == typeof(bool): return new TsType(TsTypes, type, false) { TypeName = "boolean", Declare = true };

                            case Type _ when type == typeof(string): return new TsType(TsTypes, type, false) { TypeName = "string", Declare = true };
                            case Type _ when type == typeof(Guid): return new TsType(TsTypes, type, false) { TypeName = "string", Declare = true };

                            case Type _ when type == typeof(byte): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(sbyte): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(char): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(short): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(ushort): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(int): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(uint): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(long): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(ulong): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(float): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(double): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };
                            case Type _ when type == typeof(decimal): return new TsType(TsTypes, type, false) { TypeName = "number", Declare = true };

                            case Type _ when type == typeof(DateTime): return new TsType(TsTypes, type, false) { TypeName = "Date", Declare = true };
                            case Type _ when type == typeof(DateTimeOffset): return new TsType(TsTypes, type, false) { TypeName = "Date", Declare = true };

                            case Type _ when type == typeof(object): return new TsType(TsTypes, type, false) { TypeName = "any", Declare = true };

                            case Type _ when type.IsArray: return ParseType(typeof(IEnumerable<>).MakeGenericType(type.GetElementType()));
                            case Type _ when type.IsImplement(typeof(IEnumerable<>)): return ParseType(typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments()[0]));

                            case Type _ when type.IsImplement(typeof(IDictionary<,>)): return new TsType(TsTypes, type, false) { TypeName = "any", Declare = true };

                            case Type _ when type.IsType(typeof(Nullable<>)): return TsTypes[type.GenericTypeArguments[0]].Value;

                            case Type _ when type.IsEnum: return ParseEnum(type);

                            case Type _ when type.IsClass || type.IsValueType: return ParseType(type);

                            default: throw new NotSupportedException($"{type.FullName} is not supported.");
                        }
                    };
                },
            };
        }
        public Dictionary<FieldInfo, TsConst> TsConsts { get; private set; } = new Dictionary<FieldInfo, TsConst>();

        public void WriteTo(string path, CompileOptions options = null) => File.WriteAllText(path, Compile(options));

        public string Compile(CompileOptions options = null)
        {
            var code = new StringBuilder();
            code.AppendLine(Declare.Info);

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
                                code.AppendLine($"{" ".Repeat(8)}{tsProperty.PropertyName}{(tsProperty.Required ? "" : "?")}: {typeString};");
                            }
                            code.AppendLine($"{" ".Repeat(4)}}}");

                            if (options?.OutputNames ?? false)
                            {
                                code.AppendLine($"{" ".Repeat(4)}export const enum {tsType.PureName}_names {{");
                                foreach (var tsProperty in tsType.TsProperties.Value)
                                {
                                    code.AppendLine($"{" ".Repeat(8)}{tsProperty.PropertyName} = '{tsProperty.ClrName}',");
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
                if (dType is null)
                    return type.Namespace;
                else return $"{GetTsNamespace(dType)}.{type.Name}";
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
            return new TsType(TsTypes, type, false)
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
                    return new TsType(TsTypes, type, true)
                    {
                        TypeName = $"{generics}[]",
                        Declare = true,
                    };
                }

                if (type.IsGenericTypeDefinition)
                {
                    return new TsType(TsTypes, type, true)
                    {
                        Namespace = tsNamespace,
                        TypeName = typeName,
                    };
                }
                else
                {
                    _ = TsTypes[type.GetGenericTypeDefinition()];
                    return new TsType(TsTypes, type, true)
                    {
                        Namespace = tsNamespace,
                        TypeName = typeName,
                        Declare = true,
                    };
                }
            }
            else
            {
                return new TsType(TsTypes, type, true)
                {
                    Namespace = tsNamespace,
                    TypeName = type.Name,
                };
            }
        }

        public void CacheTypes(params Type[] types) => types.Each(type => CacheType(type));
        public void CacheType<TType>() => CacheType(typeof(TType));
        public void CacheType(Type type) => _ = TsTypes[type];

        public void AddDeclaredType(Type type, string typeName) => DeclaredTypes.Add(type, typeName);

    }
}