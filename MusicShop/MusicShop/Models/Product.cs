using System.ComponentModel.DataAnnotations;

namespace MusicShop.Models
{
    public class Product
    {

        public int Id { get; set; }
        [Required]
        public  string Name { get; set; }
        [Required]
        public  string Collection { get; set; }
        [Required]
        public  string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue,ErrorMessage ="Price must be greater than 1")]
        public double Price { get; set; }
        [Required]
        public  string Image { get; set; }
        [Required]
        public Author Author { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than 1")]
        public int Count { get; set; }

    }
}
