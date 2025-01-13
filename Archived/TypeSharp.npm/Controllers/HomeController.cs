using Ajax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NStandard;
using System;
using System.Diagnostics;
using System.Text;
using TypeSharp.npm.Models;

namespace TypeSharp.npm.Controllers
{
    [TypeScriptApi]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string GetContent() => "Content";
        public string GetContent500() => throw new Exception();

        [ApiReturnFile]
        public IActionResult GetFile() => File("File Content".Pipe(Encoding.UTF8.GetBytes), "text/plain");

        [ApiReturnFile]
        public IActionResult GetFile404() => NotFound();

        [ApiReturnFile]
        public IActionResult GetFile500() => throw new Exception();

        [ApiReturnFile]
        public JSend GetJSendError() => JSend.Error("JSend error.");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
