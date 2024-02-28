using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using MusicShop.Models.ViewModels;
using System.Diagnostics;

namespace MusicShop.Controllers
{
    public class OrderController : Controller
    {
        public async Task<IActionResult> Index(string? status)
        {
            if(BDWorks.CurrentUser == null) return RedirectToAction("Register", "User");
            if (status != null)
            {
                return View(await BDWorks.GetUserOrders(BDWorks.CurrentUser.Id, status));

            }

            return View(await BDWorks.GetUserOrders(BDWorks.CurrentUser.Id, "Accepted"));
        }

        public async Task<IActionResult> AdminStatistics(string status, DateTime? minDate, DateTime? maxDate)
        {
            Debug.WriteLine(minDate);
            Debug.WriteLine(maxDate);
            if (maxDate == null) maxDate = DateTime.Now;
            OrderVM orderVM = new();
       
            orderVM.status = status;
            orderVM.MinDate = minDate;
            orderVM.MaxDate = maxDate;
         /*   Debug.WriteLine(orderVM.MinDate);
            Debug.WriteLine(orderVM.MaxDate);*/


            foreach (var item in await BDWorks.DistinctUsersIdFromOrders())
            {
                
                orderVM.result.Add(await BDWorks.GetOrdersBetweenDate(item, status, minDate, maxDate));
            }
            
            return View(orderVM);
        }

        public IActionResult Redirect(string status, DateTime minDate, DateTime maxDate)
        {
            return RedirectToAction("AdminStatistics", new { status = status, minDate = minDate, maxDate = maxDate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult UpdateDeliveryStatus( Order order)
        {
            string oldStatus = order.Status;
            var form = Request.Form;
            string status = form["status"];
            order.Status = status;



             BDWorks.EditOrder(order);


            return RedirectToAction("AdminStatistics", new { status = oldStatus});
        }
    }
}
