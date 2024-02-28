namespace MusicShop.Models
{
    public class ReservedProduct
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int UserId {  get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
