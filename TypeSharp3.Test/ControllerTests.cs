using Microsoft.AspNetCore.Mvc;
using TypeSharp.Resolvers;

namespace TypeSharp.Test;

public class ControllerTests
{
    [Fact]
    public void Test1()
    {
        var parser = new Parser(new()
        {
            Nullable = true,
            CamelCase = true,
            Resolvers =
            [
                new ControllerResolver()
                {
                    BaseAddress = "http://localhost",
                },
            ],
        })
        {
            typeof(LoginController),
        };

        var code = parser.GetCode();
    }

    public class LoginRequest<T>
    {
        public T Id { get; set; }
        public string Token { get; set; }
    }

    public class UpdateAgeRequest
    {
        public int Age { get; set; }
    }

    public class LoginController : Controller
    {
        //[HttpGet]
        //public Guid QueryId(string token)
        //{
        //    return Guid.Empty;
        //}

        [HttpPost]
        public string[] Login([FromBody] LoginRequest<Guid> model)
        {
            return ["OK."];
        }

        //[HttpPut]
        //[Route("[Controller]/user/{Action}")]
        //public void UpdateAge(string username, [FromBody] UpdateAgeRequest model)
        //{
        //}

        //[HttpPost]
        //public FileResult Download(string filename)
        //{
        //    return new();
        //}
    }
}
