namespace MusicShop.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public Product Product { get; set; }

        public int Count { get; set; }
        public int UserId { get; set; }
    }
}
