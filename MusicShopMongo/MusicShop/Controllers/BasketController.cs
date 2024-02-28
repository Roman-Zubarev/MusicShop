using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MusicShop.Models;
using MusicShop.Models.ViewModels;
using Raven.Database.Indexing;
using System.Diagnostics;

namespace MusicShop.Controllers
{
    public class BasketController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (BDWorks.CurrentUser == null) return RedirectToAction("Register", "User");

            BasketVM vm = new BasketVM();
            vm.Basket = await BDWorks.GetBasketItems();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> IncCount(ObjectId id)
        {

            await  BDWorks.IncrementCount(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecCount(ObjectId id)
        {
            await BDWorks.DecrementCount(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToBasket(ObjectId id)
        {
            if (BDWorks.CurrentUser == null) return RedirectToAction("Register", "User");

            await BDWorks.AddProductToBasket(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MakeOrder(BasketVM basketVM)
        {
            BasketVM vm = new BasketVM();
            vm.Basket = await BDWorks.GetBasketItems();


            foreach (var item in vm.Basket)
            {
                Debug.WriteLine(item.Product.Id);

                Order ord = new Order
                {
                    Count = item.Count,
                    Country = basketVM.Order.Country,
                    FirstName = basketVM.Order.FirstName,
                    LastName = basketVM.Order.LastName,
                    Address = basketVM.Order.Address,
                    PostalCode = basketVM.Order.PostalCode,
                    City = basketVM.Order.City,
                    OrderDate = basketVM.Order.OrderDate,
                    Status = basketVM.Order.Status,
                    DeliveryMethod = basketVM.Order.DeliveryMethod,
                    UserId = BDWorks.CurrentUser.Id,
                    Product = await BDWorks.GetProductById(item.Product.Id),

                };

                if (await BDWorks.MakeOrder(ord))
                {
                    BDWorks.DeleteBasketById(item.Id);
                } 

            }
          //  BDWorks.ClearBasket(BDWorks.CurrentUser);
            return RedirectToAction("Index");
        }
    }
}
