namespace TypeSharp.Units;

public class ScriptUnionType : ScriptType
{
    public UnionOperator Operator { get; set; }
    public ScriptType Left { get; set; }
    public ScriptType Right { get; set; }

    private ScriptUnionType(UnionOperator @operator, ScriptType left, ScriptType right) : base()
    {
        IsUnion = true;
        Operator = @operator;
        Left = left;
        Right = right;

        if (@operator == UnionOperator.And)
        {
            Name = $"({left.Name} & {right.Name})";
        }
        else if (@operator == UnionOperator.Or)
        {
            Name = $"({left.Name} | {right.Name})";
        }
        else throw new NotSupportedException($"Not supported operator({@operator}).");
    }

    public static ScriptUnionType And(ScriptType left, ScriptType right)
    {
        return new ScriptUnionType(UnionOperator.And, left, right);
    }

    public static ScriptUnionType Or(ScriptType left, ScriptType right)
    {
        return new ScriptUnionType(UnionOperator.Or, left, right);
    }

}
