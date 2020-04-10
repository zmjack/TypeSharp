using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSharp.Test
{
    [TypeScriptApi]
    [Route("/com/{controller}/{action}")]
    public class SimpleController : Controller
    {
        [Return(typeof(SubClass[]))]
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
        [Return(typeof(int))]
        public JsonResult GetAction(int groupId)
        {
            return Json(1);
        }

        [HttpPost]
        [Return(typeof(int))]
        public JsonResult PostAction([FromBody] RootClass model)
        {
            return Json(2);
        }

    }
}
