using Microsoft.AspNetCore.Mvc;
using TypeSharp.Test.Models;

namespace TypeSharp.Test.Controllers
{
    [TypeScriptApi]
    [Route("com/[controller]/[action=Index]")]
    public class SimpleController : Controller
    {
        [ApiReturn(typeof(SubClass[]))]
        public JsonResult GetUsers(string group)
        {
            var model = new[]
            {
                new SubClass { Name = "alice" },
                new SubClass { Name = "bob" },
            };
            return Json(model);
        }

        [HttpGet]
        [ApiReturn(typeof(int))]
        public IActionResult GetAction(int groupId)
        {
            return Json(1);
        }

        [HttpPost]
        [ApiReturn(typeof(int))]
        public IActionResult PostAction([FromBody] RootClass model)
        {
            return Json(2);
        }

        [HttpGet]
        [ApiReturnFile]
        public IActionResult GetFile(int groupId)
        {
            return NotFound();
        }

        [HttpPost]
        [ApiReturnFile]
        public IActionResult PostFile([FromBody] RootClass model)
        {
            return NotFound();
        }

    }
}
