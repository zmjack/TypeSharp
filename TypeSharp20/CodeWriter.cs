//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using System.Text.RegularExpressions;
//using TypeSharp.Build;
//using TypeSharp.Definitions;

//namespace TypeSharp
//{
//    public class CodeWriter
//    {
//        private readonly Indent _indent;

//        public List<ScriptNamespace> Namespaces = new();
//        public Dictionary<Type, ScriptClass> Classes = new();
//        public Dictionary<Type, ScriptEnum> Enums = new();
//        public Dictionary<Type, ScriptFunction> Functions = new();
//        public Dictionary<Type, ScriptType> Types = new();

//        public CodeWriter(Indent indent)
//        {
//            _indent = indent;
//        }

//        public void AddNamespaces(params ScriptNamespace[] namespaces)
//        {
//            Namespaces.AddRange(namespaces);
//        }

//        public ScriptClass GetOrCreateClass(Type clrType, Func<ScriptClass> factory)
//        {
//            if (!Classes.ContainsKey(clrType))
//            {
//                Classes.Add(clrType, factory());
//            }
//            return Classes[clrType];
//        }

//        public ScriptEnum GetOrCreateEnum(Type clrType, Func<ScriptEnum> factory)
//        {
//            if (!Enums.ContainsKey(clrType))
//            {
//                Enums.Add(clrType, factory());
//            }
//            return Enums[clrType];
//        }

//        public ScriptFunction GetOrCreateFunction(Type clrType, Func<ScriptFunction> factory)
//        {
//            if (!Functions.ContainsKey(clrType))
//            {
//                Functions.Add(clrType, factory());
//            }
//            return Functions[clrType];
//        }

//        public ScriptType GetOrCreateType(Type clrType, Func<ScriptType> factory)
//        {
//            if (!Types.ContainsKey(clrType))
//            {
//                Types.Add(clrType, factory());
//            }
//            return Types[clrType];
//        }

//        public void AddClrType(Type type)
//        {
//            var scriptType = type switch
//            {
//                _ when type == typeof(bool) => ScriptType.Boolean,
//                _ when type == typeof(string) => ScriptType.String,
//                _ when type == typeof(Guid) => ScriptType.String,
//                _ when type == typeof(byte) => ScriptType.Number,
//                _ when type == typeof(sbyte) => ScriptType.Number,
//                _ when type == typeof(char) => ScriptType.Number,
//                _ when type == typeof(short) => ScriptType.Number,
//                _ when type == typeof(ushort) => ScriptType.Number,
//                _ when type == typeof(int) => ScriptType.Number,
//                _ when type == typeof(uint) => ScriptType.Number,
//                _ when type == typeof(long) => ScriptType.Number,
//                _ when type == typeof(ulong) => ScriptType.Number,
//                _ when type == typeof(float) => ScriptType.Number,
//                _ when type == typeof(double) => ScriptType.Number,
//                _ when type == typeof(decimal) => ScriptType.Number,
//#if NET6_0_OR_GREATER
//                _ when type == typeof(DateOnly) => ScriptType.Date,
//                _ when type == typeof(DateTime) => ScriptType.Date,
//                _ when type == typeof(DateTimeOffset) => ScriptType.Date,
//#endif
//                _ when type == typeof(Array) => ScriptType.Any.MakeArrayType(),
//                _ when type.IsArray => Any.Create(() =>
//                {
//                    var scriptType = FromClrType(typeof(IEnumerable<>).MakeGenericType(type.GetElementType()));
//                    return AddClrType(type);
//                }),
//                _ => ScriptType.Any,
//            };

//            return GetOrCreateType(scriptType);
//        }

//        private readonly Regex _objNameRegex = new(@"([^`]+)");
//        private ScriptType FromClrType(Type clrType)
//        {
//            //TODO: cache constants
//            //CacheConsts(type);
//            if (clrType.IsGenericType)
//            {
//                var objName = clrType.Name.ExtractFirst(_objNameRegex);
//                var isArray = clrType.Namespace == "System.Collections.Generic" && objName == "IEnumerable";

//                if (clrType.IsGenericTypeDefinition)
//                {
//                    var generics = clrType.GetGenericArguments().Select(x => new ScriptGeneric(x.Name)).ToArray();
//                    var typeName = $"{objName}<{generics}>";

//                    if (isArray)
//                    {
//                        return GetOrCreateType(clrType, () =>
//                        {
//                            var elementGeneric = generics[0];
//                            var scriptType = new ScriptType(elementGeneric.Name, elementGeneric);
//                            var arrayType = scriptType.MakeArrayType();
//                            return arrayType;
//                        });
//                    }
//                }
//                else
//                {
//                    var definition = clrType.GetGenericTypeDefinition();
//                    return FromClrType(definition);
//                }

//                if (clrType.IsGenericTypeDefinition)
//                {
//                    return new TsType(this, clrType, true)
//                    {
//                        Namespace = tsNamespace,
//                        TypeName = typeName,
//                    };
//                }
//                else
//                {
//                    _ = TsTypes[clrType.GetGenericTypeDefinition()];
//                    return new TsType(this, clrType, true)
//                    {
//                        Namespace = tsNamespace,
//                        TypeName = typeName,
//                        Declare = true,
//                    };
//                }
//            }
//            else
//            {
//                return new TsType(this, clrType, true)
//                {
//                    Namespace = tsNamespace,
//                    TypeName = clrType.Name,
//                };
//            }
//        }
//    }
//}
