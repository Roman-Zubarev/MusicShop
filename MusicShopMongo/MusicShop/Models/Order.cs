using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MusicShop.Models
{
    public class Order
    {
        public ObjectId Id { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public  string Country {  get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public  string? LastName { get; set; }
        [Required]
        public  string Address { get; set; }
        [Required]
        public  string PostalCode { get; set; }
        [Required]
        public  string City { get; set; }
        [Required]
        public  DateTime OrderDate = DateTime.Now;
        [Required]
        public string Status = "Accepted";
        [Required]
        public  string DeliveryMethod { get; set; }
        [Required]
        public ObjectId UserId { get; set; }
        [Required]
        public Product Product { get; set; }


    }
}
