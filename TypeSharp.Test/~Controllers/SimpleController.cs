using Ajax;
using Microsoft.AspNetCore.Mvc;

namespace TypeSharp.Test
{
    [TypeScriptApi]
    [Route("com/[controller]/[action]")]
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
        public JsonResult GetAction(int groupId)
        {
            return Json(1);
        }

        [HttpPost]
        [ApiReturn(typeof(int))]
        public JsonResult PostAction([FromBody] RootClass model)
        {
            return Json(2);
        }

    }
}
