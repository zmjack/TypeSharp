using static TypeSharp.Test.GenericTests;
using static TypeSharp.Test.ModelTests;

namespace TypeSharp.Test;

public class GenericTests
{
    public interface IModel
    {
        public object Source { get; set; }
    }

    public class Model<T> : IModel
    {
        object IModel.Source { get => Source; set => throw new NotImplementedException(); }
        public T Source { get; set; }
    }

    [Fact]
    public void Test()
    {
        var _generator = new TypeScriptGenerator(new()
        {
            CamelCase = true,
            //DetectionMode = DetectionMode.AutoDetect,
        })
        {
            typeof(IModel),
            typeof(Model<string>),
        };
        var code = _generator.GetCode();
        var expected = """

""";
        Assert.Equal(expected, code);
    }
}
