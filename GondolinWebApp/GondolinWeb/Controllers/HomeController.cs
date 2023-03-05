using GondolinWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GondolinWeb.Controllers
{
    public class Home : Controller
    {
        private readonly ILogger<Home> _logger;

        public Home(ILogger<Home> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                return RedirectToAction("index", "Home", new { area = "Application" });
            }
            return View();
        }

        public IActionResult HomeSite()
        {
            //TODO: This is the area.
            if (User.Identity.IsAuthenticated == true)
            {
                return RedirectToAction("index", "Home", new { area = "Application" });
            }
            return View("Index");
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Team()
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