using Amlex.Interfaces.Services;
using Amlex.Models.Models.LegalActs;
using Amlex.Mvc.Models;
using Amlex.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Amlex.Mvc.Controllers
{
    public class HomeController(ILegalActService legalActService, IEuroVocCodesService euroVocCodesService) : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewBag.Acts = await legalActService.GetLegalActsAsync(100);
            ViewBag.EuroVocCodes = await euroVocCodesService.GetEuroVocCodesAsync();
            return View();
        }

        [Authorize]
        [HttpGet("/Details/{id:guid}")]
        public async Task<IActionResult> LegalActDetails(string id)
        {
            LegalAct? act = await legalActService.GetLegalActByIdAsync(new Guid(id));

            if (act is null)
            {
                return RedirectToAction(nameof(Error));
            }

            ViewBag.EuroVocCodes = await euroVocCodesService.GetEuroVocCodesAsync();
            return View(act);
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
