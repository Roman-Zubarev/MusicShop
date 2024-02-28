using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MusicShop.Models;
using MusicShop.Models.ViewModels;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;

namespace MusicShop.Controllers
{
    public class ProductController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
                _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? currentCollection, string? filterOption)
        {
            ProductVM productVM = new ProductVM();
            productVM.Collections = await BDWorks.GetDistinctCollections();

            if (String.IsNullOrEmpty(currentCollection)) {
               productVM.Products = await BDWorks.GetAllProducts();  
            }
            else
            {
                productVM.Products = await BDWorks.GetProductsWithCollection(currentCollection);
                productVM.CurrentCollection = currentCollection;

                Random rnd = new Random();
                string randImg = $"~/images/randomImg/rand{rnd.Next(1,4)}.jpg";               
                productVM.RandomImagePath = randImg;
                Debug.WriteLine(randImg);
            }


            if (filterOption != null)
            {
                switch (filterOption)
                {
                    case "priceDecr":
                        productVM.Products.OrderByDescending(u =>u.Price);
                        break;

                    case "priceInc":
                        productVM.Products = productVM.Products.OrderBy(u => u.Price).ToList();
                        break;
                }

            }

            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ObjectId id)
        {
            if (!await BDWorks.CheckProductIdExistence(id))
            {
                var objFromDb = await BDWorks.GetProductById(id);
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WC.ImagePath;
                var oldFile = Path.Combine(upload, objFromDb.Image);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }

                BDWorks.DeleteProductById(id);
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("ProductCard", "Product", new { id = id, error = "Product has dependencies" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(ObjectId id)
        {

            Product product = await BDWorks.GetProductById(id);
            var list = await BDWorks.GetAllAuthors();

            ProductVM productVM = new();

           productVM.AuthorIdNameHashTabl = list.ToDictionary(item => item.Id,
                             item => item.Name);

            productVM.Name = product.Name;
            productVM.Price = product.Price;
            productVM.Description = product.Description;
            productVM.Image = product.Image;
            productVM.Id = product.Id;
            productVM.Collection = product.Collection;
            productVM.Author = product.Author;
            productVM.Count = product.Count;
            if (productVM != null)
            {
                return View(productVM);

            }
            return NotFound();

        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> EditPost(ProductVM product)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath ;

            Product objFromDb = await BDWorks.GetProductById(product.Id);
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
                product.Image = fileName + extension;
            }
            else
            {
                product.Image = objFromDb.Image;
            }

            BDWorks.EditProduct(product);

            return RedirectToAction("ProductCard", new { id = product.Id});
        }

        [HttpGet]
        public  async Task<IActionResult> Insert()
        {
       
            var list = await BDWorks.GetAllAuthors();
            ProductVM productVM = new ProductVM();

           productVM.AuthorIdNameHashTabl = list.ToDictionary(item => item.Id,
                             item => item.Name);

            return View(productVM);
        }

        [HttpPost, ActionName("Insert")]
        [ValidateAntiForgeryToken]
        public IActionResult InsertPost(ProductVM product)
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

            product.Image = fileName + extension;

            BDWorks.InsertProduct(product);

            //  return RedirectToAction("ProductCard", new { id = product.Id });

            return RedirectToAction("Index");

        }




        public async Task<IActionResult> ProductCard(ObjectId id, string? error)
        {
            if (error != null)
            {
                ViewBag.Message = error;
            }
            Product product = await BDWorks.GetProductById(id);

            if (product != null)
            {
                return View(product);

            }
            return NotFound();

        }

    }
}
