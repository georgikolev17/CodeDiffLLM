using System.Diagnostics;
using CodeDiffLLM.Models;
using CodeDiffLLM.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeDiffLLM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICodeService codeService;

        public HomeController(ILogger<HomeController> logger, ICodeService codeService)
        {
            _logger = logger;
            this.codeService = codeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string originalCode, string modifiedCode)
        {
            var diffLines = this.codeService.GetCodeDiffLines(originalCode, modifiedCode);
            
            string prompt = this.codeService.BuildPrompt(diffLines);

            return View("ShowPrompt", prompt);
        }

        public IActionResult ShowPrompt(string prompt)
        {
            return View(prompt);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
