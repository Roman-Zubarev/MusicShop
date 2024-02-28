using MongoDB.Bson;

namespace MusicShop.Models
{
    public class Basket
    {
        public ObjectId Id { get; set; }
        public Product Product { get; set; }

        public int Count { get; set; }
        public ObjectId UserId { get; set; }
    }
}
