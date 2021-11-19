using Microsoft.AspNetCore.Mvc;
using TypeSharp.React.Models;

namespace TypeSharp.React.Controllers.Identity
{
    public class IdentityAppController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "IdentityApp";
            return View("/scripts/views/default.cshtml", new RenderModel
            {
                Area = RouteData.Values["area"]?.ToString(),
                Controller = RouteData.Values["controller"]?.ToString(),
                Action = RouteData.Values["action"]?.ToString(),
            });
        }

    }
}