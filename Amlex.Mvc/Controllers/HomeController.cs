using Amlex.Interfaces.Services;
using Amlex.Mvc.Models;
using Amlex.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Amlex.Mvc.Controllers
{
    [Authorize]
    public class HomeController(ILegalActService legalActService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Acts = await legalActService.GetLegalActsAsync(100);
            return View();
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
