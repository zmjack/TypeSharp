using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public record ScriptType : INameable, IEquatable<ScriptType>
    {
        public static readonly ScriptType Undefined = new("undefined");
        public static readonly ScriptType Null = new("null");
        public static readonly ScriptType Never = new("never");
        public static readonly ScriptType Number = new("number");
        public static readonly ScriptType String = new("string");
        public static readonly ScriptType Boolean = new("boolean");
        public static readonly ScriptType Date = new("Date");
        public static readonly ScriptType Any = new("any");
        public static readonly ScriptType Unknown = new("unknown");

        public string Namespace { get; }
        public string Name { get; internal set; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, Name);

        public ScriptUnderlayingType UnderlayingType { get; internal set; }
        public object Underlaying { get; internal set; }
        public bool IsUnion { get; internal set; }
        public bool Array { get; internal set; }

        internal ScriptType()
        {
        }

        private ScriptType(string name)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.BuiltInType;
        }

        public ScriptType MakeArrayType()
        {
            return new ScriptType($"{Name}[]", this)
            {
                Array = true,
            };
        }

        public ScriptType(string name, ScriptGeneric generic)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Generic;
            Underlaying = generic;
        }

        public ScriptType(string name, ScriptType type)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Type;
            Underlaying = type;
        }

        public ScriptType(string name, ScriptInterface @interface)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Interface;
            Underlaying = @interface;
        }

        public ScriptType(string name, ScriptEnum @enum)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Enum;
            Underlaying = @enum;
        }

        public ScriptType(string name, ScriptClass @class)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Class;
            Underlaying = @class;
        }

        public ScriptType(string name, ScriptMethod method)
        {
            Name = name;
            UnderlayingType = ScriptUnderlayingType.Method;
            Underlaying = method;
        }
    }
}
