using Microsoft.AspNetCore.Mvc;
using TypeSharp.Resolvers;

namespace TypeSharp.Test;

public class ControllerTests
{
    [Fact]
    public void Test1()
    {
        var parser = new TypeScriptGenerator(new()
        {
            CamelCase = true,
            DetectionMode = DetectionMode.AutoDetect,
            ModuleCode = ModuleCode.Nested,
            Resolvers =
            [
                new ControllerResolver()
                {
                    BaseAddress = "http://localhost",
                },
            ],
        })
        {
        };

        var code = parser.GetCode();
    }

    public enum HttpStatus
    {
        None,
        OK = 200,
        NotFound = 404,
    }

    public class LoginRequest<T>
    {
        public HttpStatus Status { get; set; }
        public T Id { get; set; }
        public int? Token { get; set; }
    }

    [TypeScriptGenerator]
    public class LabelValueNode<TValue>
    {
        public LabelValueNode<TValue>[] Children { get; set; }
    }

    public class UpdateAgeRequest
    {
        public int? Age { get; set; }
    }

    public class LoginController : Controller
    {
        //[HttpGet]
        //public Guid QueryId(string token)
        //{
        //    return Guid.Empty;
        //}

        [HttpPut(Name = "userlogin")]
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
