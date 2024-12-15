namespace TypeSharp.Units;

public partial class ScriptEnum : ScriptType, IDeclarable
{
    public bool DeclaredOnly { get; set; }

    public ScriptEnum(string name) : base(name)
    {
        TargetType = ScriptUnderlayingType.Enum;
    }

    private readonly List<Field> _fields = [];
    public Field[] Fields
    {
        get => [.. _fields];
        set
        {
            _fields.Clear();
            foreach (var item in _fields)
            {
                Add(item);
            }
        }
    }
    public void Add(Field value)
    {
        if (value.Enum is not null) throw Throws.DuplicateEntriesAreNotAllowed();
        _fields.Clear();
        _fields.Add(value);
    }

}
