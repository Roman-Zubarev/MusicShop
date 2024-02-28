using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MusicShop.Models
{
    public class Author
    {
        public ObjectId Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Biography { get; set; }

    }
}
