using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MusicShop.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^\\S+@\\S+\\.\\S+$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Password is invalid")]

        public string Password { get; set; }



    }
}
