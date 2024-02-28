namespace MusicShop.Models.ViewModels
{
    public class OrderVM
    {
        public List<List<Order>> result = new();

        public DateTime? MinDate = DateTime.MinValue;
        public DateTime? MaxDate = DateTime.Now;

        public string status { get; set; }

    }
}
