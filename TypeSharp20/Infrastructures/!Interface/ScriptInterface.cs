using System.Text;
using static TypeSharp.Infrastructures.ScriptClass;

namespace TypeSharp.Infrastructures;

public partial record ScriptInterface : IComment, IGenericGenerable, IEncodable, INameable
{
    public static readonly ScriptInterface Generator = new("Generator",
    [
        new("T", ScriptType.Unknown),
        new("TReturn", ScriptType.Any),
        new("TNext", ScriptType.Unknown),
    ], []);

    public ScriptNamespace? Namespace { get; internal set; }
    public string Name { get; }
    public QualifiedName FullName => new(Namespace?.Name, $"{Name}{(GenericArguments is not null ? $"<{GenericArguments.Select(x => x.Name).Join(", ")}>" : string.Empty)}");
    public ScriptGeneric[] Generics { get; set; }
    public ScriptType[] GenericArguments { get; set; }

    public ScriptInterface MakeGenericInterface(params ScriptType[] arguments)
    {
        if (Generics is null) throw new InvalidOperationException("No generic arguments definition.");

        var least = Generics.TakeWhile(x => x.Default is null).Count();
        var most = Generics.Length;
        var inputLength = arguments.Length;

        if (inputLength < least) throw new ArgumentException($"Too less arguments. (At least {least} arguments are required.)", nameof(arguments));
        if (inputLength > Generics.Length) throw new ArgumentException($"Too more arguments specified. (At most {most} arguments are required.)", nameof(arguments));

        var types = Generics.Select(x => x.Default).ToArray();
        foreach (var (index, type) in arguments.Pairs())
        {
            types[index] = type;
        }

        return this with
        {
            GenericArguments = types,
        };
    }

    public string Comment { get; set; }
    public ScriptType[] ExtendedInterfaces { get; set; }

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
        if (value.Interface is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        value.Interface = this;
        _fields.Add(value);
    }

    public ScriptInterface(string name)
    {
        Name = name;
    }
    public ScriptInterface(string name, ScriptGeneric[] generics, ScriptType[] extends)
    {
        Name = name;
        Generics = generics ?? [];
        ExtendedInterfaces = extends ?? [];
    }

    public string Encode(Indent indent, string ownerPrefix)
    {
        if (Name.IsNullOrWhiteSpace()) throw new InvalidOperationException($"{nameof(Name)} is required.");

        var sb = new StringBuilder();
        sb.AppendLine($"{indent}interface {Name}{(Generics is not null ? $"<{Generics.Select(x => x.Name).Join(", ")}>" : string.Empty)}");
        sb.AppendLine($"{indent}{{");

        if (Fields is not null)
        {
            foreach (var property in Fields)
            {
                var propertyType = property.Type.FullName.GetSimplifiedName(ownerPrefix);
                sb.AppendLine($"{indent.Tab()}{property.Name}: {propertyType};");
            }
        }

        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }
}