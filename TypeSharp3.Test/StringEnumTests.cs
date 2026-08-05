namespace TypeSharp.Test;

public class StringEnumTests
{
    public static class TypeEnum
    {
        public const string Class = "class";
        public const string Struct = "struct";
    }

    [Fact]
    public void Test()
    {
        var _generator = new TypeScriptGenerator(new()
        {
            CamelCase = true,
        })
        {
            typeof(TypeEnum),
        };
        var code = _generator.GetCode();
        var expected = """

""";
        Assert.Equal(expected, code);
    }
}
