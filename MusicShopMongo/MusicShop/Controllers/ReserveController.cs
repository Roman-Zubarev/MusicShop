using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MusicShop.Models;
using MusicShop.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MusicShop.Controllers
{
    public class ReserveController : Controller
    {

        public async Task<IActionResult> Index()
        {
            if (BDWorks.CurrentUser == null) return RedirectToAction("Register", "User");

            await BDWorks.CheckExpiratedReservations();
            ReservedProductVM reservedProductVM = new();

            reservedProductVM.ReservedProducts = await BDWorks.GetUsersReservedItems();
            return View(reservedProductVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Reserve(ObjectId id)
        {
            if (BDWorks.CurrentUser == null) return RedirectToAction("Register", "User");
            if (!await BDWorks.СheckReservedItems(BDWorks.CurrentUser.Id, id))
            {
                return RedirectToAction("ProductCard", "Product", new { id = id, error = "Alredy reserved" });

            }

            await  BDWorks.InsertReservedProduct(id);

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> MakeOrder(ReservedProductVM resVm)
        {
            ReservedProductVM vm = new ReservedProductVM();
            vm.ReservedProducts = await BDWorks.GetUsersReservedItems();


            foreach (var item in vm.ReservedProducts)
            {
                BDWorks.DeleteReservedProductById(item.Id);

                Order ord = new Order
                {
                    Count = 1,
                    Country = resVm.Order.Country,
                    FirstName = resVm.Order.FirstName,
                    LastName = resVm.Order.LastName,
                    Address = resVm.Order.Address,
                    PostalCode = resVm.Order.PostalCode,
                    City = resVm.Order.City,
                    OrderDate = resVm.Order.OrderDate,
                    Status = resVm.Order.Status,
                    DeliveryMethod = resVm.Order.DeliveryMethod,
                    UserId = item.UserId,
                    Product = await BDWorks.GetProductById(item.Product.Id),

                };

                await  BDWorks.MakeOrder(ord);

            }
            //BDWorks.ClearBasket(BDWorks.CurrentUser);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteReserve(ObjectId id)
        {

            BDWorks.DeleteReservedProductById(id);

           
            if (BDWorks.IsAdmin) return RedirectToAction("Index", "Statistics");

            return RedirectToAction("Index");
        }
    }
}
