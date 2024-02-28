using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using System.Diagnostics;
using System.Formats.Asn1;

namespace MusicShop.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Register()
        {
            return View("RegisterLogin");
        }

        [ActionName("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(User user)
        {

            if (await BDWorks.CheckUserExist(user.Email) && user.Email != "admin@gmail.com")
            {
                BDWorks.RegisterUser(user);
            }
            else
            {
                ViewBag.Message = "User with that email exists";
            }
            return View("RegisterLogin");
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {

      
            if  (!await BDWorks.LogInUser(user))
            {
                ViewBag.Message = "Incorrect email or password";
                return View("RegisterLogin");
            }
            

            return RedirectToAction("Index","Product");
        }
    }
}
