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
            DetectionMode = DetectionMode.None,
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
            typeof(LoginRequest<>),
        };

        var code = parser.GetCode();
    }

    public enum HttpStatus
    {
        None,
        OK = 200,
        NotFound = 404,
    }

    public class Tag
    {
    }

    [TypeScriptGenerator]
    public interface ILoginRequest<T>
    {
        HttpStatus Status { get; set; }
        T Id { get; set; }
        string Name { get; set; }
        int? Token { get; set; }
    }

    public class LoginRequest<T> : ILoginRequest<T>
    {
        public HttpStatus Status { get; set; }
        public T Id { get; set; }
        public string Name { get; set; }
        public int? Token { get; set; }
        public required Tag ReqTag { get; set; }
        public Tag? Tag { get; set; }
        public required Dictionary<string, string> ReqDict { get; set; }
        public Dictionary<string, string>? Dict { get; set; }
    }

    public class LabelValueNode<TValue>
    {
        public Dictionary<string, string> Children { get; set; }
    }

    public class UpdateAgeRequest
    {
        public int? Age { get; set; }
    }

    public class LoginController : Controller
    {
        [HttpGet]
        public IEnumerable<UpdateAgeRequest> ReturnEnumerable()
        {
            return [];
        }

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
