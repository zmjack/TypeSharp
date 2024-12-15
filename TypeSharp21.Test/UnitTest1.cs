using NStandard;
using TypeSharp21.AST;

namespace TypeSharp21.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var source = new SourceFile
        {
            Statements =
            [
                new VariableStatement
                {
                    DeclarationList = new()
                    {
                        Flags = SyntaxFlags.Const,
                        Declarations =
                        [
                            new("a", new NumericLiteral(1)),
                        ],
                    },
                }
            ]
        };

        var code = source.GetText();
        Assert.Equal(
            """
            const a = 1;
            """,
            code
        );
    }

    [Fact]
    public void Test2()
    {
        var source = new SourceFile
        {
            Statements =
            [
                new InterfaceDeclaration("IPerson")
                {
                    Members =
                    [
                        new PropertySignature("name", StringKeyword.Default),
                        new GetAccessor("age", NumberKeyword.Default),
                        new SetAccessor("age", new("n", NumberKeyword.Default)),
                    ]
                }
            ]
        };

        var code = source.GetText();
        Assert.Equal(
            """
            interface IPerson {
                name: string;
                get age(): number;
                set age(n: number);
            }
            """,
            code
        );
    }

    [Fact]
    public void Test3()
    {
        var source = new SourceFile
        {
            Statements =
            [
                new InterfaceDeclaration("IPerson")
                {
                    TypeParameters =
                    [
                        new TypeParameter("T")
                        {
                            Constraint = StringKeyword.Default,
                        }.Pipe(out var g_T)
                    ],
                    Members =
                    [
                        new PropertySignature("name", new TypeReference(g_T.Name)),
                        new GetAccessor("age", NumberKeyword.Default),
                        new SetAccessor("age", new("n", NumberKeyword.Default)),
                    ]
                }
            ]
        };

        var code = source.GetText();
        Assert.Equal(
            """
            interface IPerson<T extends string> {
                name: T;
                get age(): number;
                set age(n: number);
            }
            """,
            code
        );
    }
}
