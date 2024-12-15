using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeSharp.DevAnalyzer;

internal class CodeWriter
{
    public struct TypeInfo
    {
        public string Name { get; set; }
        public string? DeclaredType { get; set; }
        public string[] Namespaces { get; set; }
    }

    public struct WriterParams
    {
        public Indent Indent { get; set; }
        public string Name { get; set; }
        public string? DeclaredType { get; set; }
        public string[] Namespaces { get; set; }
    }
    public delegate void InnerActionDelegate(WriterParams @params);

    private readonly GeneratorExecutionContext _context;

    public CodeWriter(GeneratorExecutionContext context)
    {
        _context = context;
    }

    public void WriteNamespaces(StringBuilder builder, Indent indent, string[] namespaces, Action<Indent> action)
    {
        if (namespaces.Any())
        {
            builder.AppendLine($"{indent}namespace {string.Join(".", namespaces)}");
            builder.AppendLine($"{indent}{"{"}");
            action(indent + 1);
            builder.AppendLine($"{indent}{"}"}");
        }
        else
        {
            action(indent);
        }
    }

    public void Write(StringBuilder builder, Indent indent, string fullName, InnerActionDelegate action)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return;

        var typeInfo = ExtractTypeInfo(fullName);
        var rootNamespaces = Config.RootNamespace.Split('.');

        WriteNamespaces(builder, indent, rootNamespaces, indent =>
        {
            WriteNamespaces(builder, indent, typeInfo.Namespaces, indent =>
            {
                var namespaces = typeInfo.Namespaces;
                var declaredType = typeInfo.DeclaredType;
                var name = typeInfo.Name;

                if (declaredType is not null)
                {
                    builder.AppendLine(
                        $"""
                        {indent}public partial class {declaredType}
                        {indent}{"{"}
                        """
                    );
                }

                action(new()
                {
                    Indent = declaredType is not null ? indent + 1 : indent,
                    Name = name!,
                    DeclaredType = declaredType,
                    Namespaces = [.. rootNamespaces, .. namespaces]
                });

                if (declaredType is not null)
                {
                    builder.AppendLine($"{indent}{"}"}");
                }
            });
        });
    }

    public TypeInfo ExtractTypeInfo(string fullName)
    {
        var namespaces = new List<string>();
        var names = fullName!.Split('.');
        var classIndex = names.Length - 2;
        var interfaceIndex = names.Length - 1;

        string? declaredType = null;
        string? name = null;
        var i = 0;
        foreach (var _name in names)
        {
            if (i == interfaceIndex)
            {
                name = _name;
            }
            else if (i == classIndex)
            {
                declaredType = _name;
            }
            else
            {
                namespaces.Add(_name);
            }
            i++;
        }

        return new()
        {
            DeclaredType = declaredType,
            Name = name!,
            Namespaces = [.. namespaces],
        };
    }
}
