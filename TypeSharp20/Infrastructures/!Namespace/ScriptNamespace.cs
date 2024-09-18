using System.Text;

namespace TypeSharp.Infrastructures;

public class ScriptNamespace : INameable, IEncodable
{
    public ScriptNamespace? SuperNamespace { get; set; }
    public string Name { get; }
    public QualifiedName FullName => new(SuperNamespace?.Name, Name);

    public ScriptNamespace(string name)
    {
        if (name.IsNullOrWhiteSpace()) throw Throws.NameIsRequired();
        Name = name;
    }

    private readonly List<ScriptNamespace> _namespaces = [];
    public ScriptNamespace[] Namespaces
    {
        get => [.. _namespaces];
        set
        {
            _namespaces.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(ScriptNamespace value)
    {
        if (value.SuperNamespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.SuperNamespace = this;
        _namespaces.Add(value);
    }

    private readonly List<ScriptEnum> _enums = [];
    public ScriptEnum[] Enums
    {
        get => [.. _enums];
        set
        {
            _enums.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(ScriptEnum value)
    {
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Namespace = this;
        _enums.Add(value);
    }

    private readonly List<ScriptClass> _classes = [];
    public ScriptClass[] Classes
    {
        get => [.. _classes];
        set
        {
            _classes.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(ScriptClass value)
    {
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Namespace = this;
        _classes.Add(value);
    }

    private readonly List<ScriptInterface> _interfaces = [];
    public ScriptInterface[] Interfaces
    {
        get => [.. _interfaces];
        set
        {
            _interfaces.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(ScriptInterface value)
    {
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Namespace = this;
        _interfaces.Add(value);
    }

    private readonly List<ScriptType> _types = [];
    public ScriptType[] Types
    {
        get => [.. _types];
        set
        {
            _types.Clear();
            foreach (var item in value)
            {
                Add(item);
            }
        }
    }
    public void Add(ScriptType value)
    {
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Namespace = this;
        _types.Add(value);
    }

    private readonly List<ScriptFunction> _functions = [];
    public ScriptFunction[] Functions
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
    public void Add(ScriptFunction value)
    {
        if (value.Namespace is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Namespace = this;
        _functions.Add(value);
    }

    public string Encode(Indent indent, string ownerPrefix)
    {
        if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

        var sb = new StringBuilder();
        sb.AppendLine($"{indent}namespace {Name}");
        sb.AppendLine($"{indent}{{");

        if (Classes is not null)
        {
            foreach (var @class in Classes)
            {
                sb.AppendLine(@class.Encode(indent.Tab(), ownerPrefix));
            }
        }

        sb.AppendLine($"}}");

        return sb.ToString();
    }

}
