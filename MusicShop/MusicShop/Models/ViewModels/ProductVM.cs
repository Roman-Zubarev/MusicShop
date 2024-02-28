using System.Collections;

namespace MusicShop.Models.ViewModels
{
    public class ProductVM : Product
    {
        public List<Product> Products { get; set; }

        public List<string> Collections { get; set; }

        public string CurrentCollection = "";

        public string AuthorID { get; set; }

        public Dictionary<int, string> AuthorIdNameHashTabl { get; set; }
        public string RandomImagePath = "~/images/randomImg/rand1.jpg";


        /*       public ProductVM(Product? product)
               {
                   this.Name = product.Name;
                   this.Price = product.Price;
                   this.Description = product.Description;
                   this.Image = product.Image;
                   this.Id = product.Id;
                   this.Collection = product.Collection;
               }*/

    }
}
