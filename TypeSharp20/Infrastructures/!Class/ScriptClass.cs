using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TypeSharp.Infrastructures;

public partial record ScriptClass : IDeclarable, INameable, IGenericGenerable, IEncodable
{
    public bool DeclaredOnly { get; set; }

    public ScriptNamespace? Namespace { get; internal set; }
    public string Name { get; }
    public QualifiedName FullName => new(Namespace?.Name, Name);
    public ScriptType[] GenericArguments { get; set; } = [];

    public ScriptClass? InheritedClass { get; set; }
    public ScriptType[] ImplementedTypes { get; set; }

    private readonly List<Field> _fields = [];
    public Field[] Fields
    {
        get => [.. _fields];
        set
        {
            _fields.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(Field value)
    {
        if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Class = this;
        _fields.Add(value);
    }

    private readonly List<Getter> _getters = [];
    public Getter[] Getters
    {
        get => [.. _getters];
        set
        {
            _getters.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(Getter value)
    {
        if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Class = this;
        _getters.Add(value);
    }

    private readonly List<Setter> _setters = [];
    public Setter[] Setters
    {
        get => [.. _setters];
        set
        {
            _setters.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(Setter value)
    {
        if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Class = this;
        _setters.Add(value);
    }

    private readonly List<Function> _functions = [];
    public Function[] Functions
    {
        get => [.. _functions];
        set
        {
            _functions.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(Function value)
    {
        if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Class = this;
        _functions.Add(value);
    }

    public ScriptClass(string name)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        ImplementedTypes = [];
    }
    public ScriptClass(string name, ScriptClass? extends, ScriptType[] implements)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        InheritedClass = extends;
        ImplementedTypes = implements ?? [];
    }

    public string Encode(Indent indent, string ownerPrefix)
    {
        var inheritedClass = InheritedClass?.FullName.GetSimplifiedName(ownerPrefix);
        var implements = ImplementedTypes?.Select(x => x.FullName.GetSimplifiedName(ownerPrefix));

        var sb = new StringBuilder();
        sb.AppendLine($"{indent}class {Name}{(inheritedClass is not null ? $" {inheritedClass}" : string.Empty)}{(implements is not null ? $" implements {implements.Join(", ")}" : string.Empty)}");
        sb.AppendLine($"{indent}{{");

        foreach (var field in _fields)
        {
            var code = field.Encode(indent.Tab(), FullName.Value);
            sb.AppendLine(code);
        }
        if (_fields.Any()) sb.AppendLine();

        foreach (var getter in _getters)
        {
            var code = getter.Encode(indent.Tab(), FullName.Value);
            sb.AppendLine(code);
        }
        foreach (var setter in _setters)
        {
            var code = setter.Encode(indent.Tab(), FullName.Value);
            sb.AppendLine(code);
        }

        foreach (var function in _functions)
        {
            var code = function.Encode(indent.Tab(), FullName.Value);
            sb.AppendLine(code);
        }

        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }

}
