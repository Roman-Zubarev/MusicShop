using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using MusicShop.Models.ViewModels;

namespace MusicShop.Controllers
{
    public class StatisticsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if(!BDWorks.IsAdmin)
            {
                return RedirectToAction("Index","Home");
            }

            BDWorks.UpdateStatistics();

            StatisticsVM statisticsVM = new StatisticsVM();
            statisticsVM.StatisticsList = await BDWorks.GetStatistics();
            statisticsVM.TopTen = await BDWorks.GetTenTopProducts();
            statisticsVM.AllReservedProducts = await BDWorks.GetAllReservedProducts();
            return View(statisticsVM);
        }
    }
}
