using MongoDB.Bson;

namespace MusicShop.Models
{
    public class ReservedProduct
    {
        public ObjectId Id { get; set; }
        public Product Product { get; set; }
        public ObjectId UserId {  get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
