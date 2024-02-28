using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using System.Diagnostics;

namespace MusicShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
           // await BDWorks.LogInUser(new User { Email = "rvz031003@gmail.com", Password = "123@Qwrqew" });
           BDWorks.IsAdmin = true;
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