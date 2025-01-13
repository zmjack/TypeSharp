using System.Text;

namespace TypeSharp.Units;

public partial class ScriptClass : ScriptType, IDeclarable, INameable, IEncodable
{
    public bool DeclaredOnly { get; set; }

    public ScriptGeneric[] Generics { get; }
    public ScriptClass? Extends { get; }
    public ScriptType[] Implements { get; }

    public ScriptClass(string name)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Generics = [];
        Implements = [];
    }
    public ScriptClass(string name, ScriptClass? extends = null, ScriptType[]? implements = null)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Generics = [];
        Extends = extends;
        Implements = implements ?? [];
    }
    public ScriptClass(string name, ScriptGeneric[] generics, ScriptClass? extends = null, ScriptType[]? implements = null)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
        Generics = generics ?? [];
        Extends = extends;
        Implements = implements ?? [];
    }

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
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        if (value.Class is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Class = this;
        _functions.Add(value);
    }

    public string Encode()
    {
        return Encode(Indent.Zero, Namespace?.FullName);
    }

    public string Encode(Indent indent, string? ownerPrefix)
    {
        var inheritedClass = Extends?.FullName.GetSimplifiedName(ownerPrefix);
        var implements = Implements?.Select(x => x.FullName.GetSimplifiedName(ownerPrefix)).ToArray();

        var sb = new StringBuilder();
        sb.Append($"{indent}class {Name}");

        if (Generics is not null && Generics.Length > 0)
        {
            sb.Append($"<{Generics.Select(x => x.Name).Join(", ")}>");
        }
        if (inheritedClass is not null)
        {
            sb.Append($" extends {inheritedClass}");
        }
        if (implements is not null && implements.Length > 0)
        {
            sb.Append($" implements {implements.Join(", ")}");
        }
        sb.AppendLine();

        sb.AppendLine($"{indent}{{");

        foreach (var field in _fields)
        {
            var code = field.Encode(indent.Tab(), FullName);
            sb.AppendLine(code);
        }
        if (_fields.Any()) sb.AppendLine();

        foreach (var getter in _getters)
        {
            var code = getter.Encode(indent.Tab(), FullName);
            sb.AppendLine(code);
        }
        foreach (var setter in _setters)
        {
            var code = setter.Encode(indent.Tab(), FullName);
            sb.AppendLine(code);
        }

        foreach (var function in _functions)
        {
            var code = function.Encode(indent.Tab(), FullName);
            sb.AppendLine(code);
        }

        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }

}
