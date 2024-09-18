using System.Text;
using TypeSharp.Infrastructures;

namespace TypeSharp.Test;

public class TypeSharp20Tests
{
    [Fact]
    public void Test()
    {
        var code = new StringBuilder();
        code.AppendLine();

        var iPerson = new ScriptInterface("IPerson")
        {
            Fields =
            [
                new(Access.Public, "firstName", ScriptType.String),
                new(Access.Public, "lastName", ScriptType.Number),
            ]
        };
        code.AppendLine(iPerson.Encode(Indent.Zero, iPerson.Namespace?.FullName.Value));

        var iCity = new ScriptInterface("ICity")
        {
            Fields =
            [
                new(Access.Public, "city", ScriptType.String),
            ]
        };
        code.AppendLine(iCity.Encode(Indent.Zero, iCity.Namespace?.FullName.Value));

        var root = new ScriptClass("Root", null,
        [
            new(iPerson.Name, iPerson),
            new(iCity.Name, iCity)
        ])
        {
            Fields =
            [
                new(Access.Public, "firstName", ScriptType.String),
                new(Access.Public, "lastName", ScriptType.Number),
            ],
            Getters =
            [
                new(Access.Public, "nickName", ScriptType.String,
                """
                var s = '123'
                return s;
                """),
            ],
            Setters =
            [
                new(Access.Public, "nickName", ScriptType.String,
                """
                console.log(value);
                """),
            ],
            Functions =
            [
                new(Access.Public, "func", ScriptType.String,
                [
                    new("name", ScriptType.String)
                ],
                """
                console.log(name);
                """),

                new(Access.Public, "func2", ScriptType.String,
                [
                    new("name", ScriptType.String)
                ],
                """
                console.log(name);
                """),
            ],
        };

        code.AppendLine(root.Encode(Indent.Zero, "Root"));
        var cccc = code.ToString();
    }
}
