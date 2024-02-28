using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MusicShop.Models;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicShop.Controllers
{
    public class AuthorController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthorController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {          
            return View( await BDWorks.GetAllAuthors());
        }
        [HttpGet]
        public async Task<IActionResult> AuthorCard(ObjectId id, string? error)
        {
            if (error != null)
            {
                ViewBag.Message = error;
            }
            Author product = await BDWorks.GetAuthorById(id);

            if (product != null)
            {
                return View(product);

            }
            return NotFound();
        }


        [HttpGet]
        public IActionResult Insert()
        {
            return View();
        }

        [HttpPost, ActionName("Insert")]
        [ValidateAntiForgeryToken]
        public IActionResult InsertPost(Author author)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            string upload = webRootPath + WC.ImagePath;
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(files[0].FileName);
            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }

            author.Image = fileName + extension;

            BDWorks.InsertAuthor(author);
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ObjectId id)
        {

            if (await BDWorks.CheckProductDependencies(id))
            {
                Author objFromDb = await BDWorks.GetAuthorById(id);

                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WC.ImagePath;
                var oldFile = Path.Combine(upload, objFromDb.Image);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                BDWorks.DeleteAuthorById(id);

                return RedirectToAction("Index");
            }
            return RedirectToAction("AuthorCard", "Author", new { id = id, error = "Author has dependencies" });
        }




        [HttpGet]
        public async Task<IActionResult> Edit(ObjectId id)
        {
            var author = await BDWorks.GetAuthorById(id);

            if (author != null)
            {
                return View(author);
            }
            return NotFound();

        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Author author)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            var objFromDb = await BDWorks.GetAuthorById(author.Id);
            if (files.Count > 0)
            {
                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                var oldFile = Path.Combine(upload, objFromDb.Image);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                author.Image = fileName + extension;
            }
            else
            {
                author.Image = objFromDb.Image;
            }

            BDWorks.EditAuthor(author);

            return RedirectToAction("AuthorCard", new { id = author.Id });
        }
    }
}
