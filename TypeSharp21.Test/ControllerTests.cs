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
            Resolvers =
            [
                new ControllerResolver()
                {
                    Module = "api",
                    BaseAddress = "http://localhost",
                },
            ],
        })
        {
            typeof(LoginController),
        };

        var code = parser.GetCode();
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    internal class LoginController : ControllerBase
    {
        [HttpPost]
        public string Login([FromBody] LoginRequest model)
        {
            return "OK.";
        }

        [HttpGet]
        public int UserInfo(string username)
        {
            return 1;
        }
    }
}
