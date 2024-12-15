namespace TypeSharp21.Test;

public class ClrClassTest
{
    class Person
    {
        public string Name { get; set; }
    }

    class Student
    {
        public Person Person { get; set; }
        public string Grade { get; set; }
    }


    [Fact]
    public void Test1()
    {
        var parser = new TypeScriptParser()
        {
            typeof(Person),
            typeof(Student),
        };

        var code = parser.GetCode();
        Assert.Equal(
            """
            interface Student {
                Person: Person;
                Grade: string;
            }
            interface Person {
                Name: string;
            }
            """,
            code
        );
    }
}
