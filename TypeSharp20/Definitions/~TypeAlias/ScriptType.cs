using System;
using System.Collections.Generic;
using System.Text;
using TypeSharp.Interfaces;

namespace TypeSharp.Definitions
{
    public class ScriptType : INameable, IEquatable<ScriptType>
    {
        public static readonly ScriptType Undefined = new("undefined");
        public static readonly ScriptType Null = new("null");
        public static readonly ScriptType Never = new("never");
        public static readonly ScriptType Number = new("number");
        public static readonly ScriptType String = new("string");

        public string Namespace { get; }
        public string Name { get; internal set; }
        public QualifiedName FullName => QualifiedName.Combine(Namespace, Name);

        public ScriptTypeWrapper WrapperType { get; internal set; }
        public object Definition { get; internal set; }
        public bool IsUnion { get; internal set; }

        internal ScriptType()
        {
        }

        private ScriptType(string name)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.BuiltInType;
        }

        public ScriptType(string name, ScriptGeneric generic)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Generic;
            Definition = generic;
        }

        public ScriptType(string name, ScriptType type)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Type;
            Definition = type;
        }

        public ScriptType(string name, ScriptInterface @interface)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Interface;
            Definition = @interface;
        }

        public ScriptType(string name, ScriptEnum @enum)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Enum;
            Definition = @enum;
        }

        public ScriptType(string name, ScriptClass @class)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Class;
            Definition = @class;
        }

        public ScriptType(string name, ScriptMethod method)
        {
            Name = name;
            WrapperType = ScriptTypeWrapper.Method;
            Definition = method;
        }

        public bool Equals(ScriptType other)
        {
            return Definition == other.Definition;
        }
    }
}
