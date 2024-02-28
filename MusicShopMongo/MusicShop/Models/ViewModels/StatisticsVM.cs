namespace MusicShop.Models.ViewModels
{
    public class StatisticsVM
    {
        public List<Statistics> StatisticsList { get; set; }

        public Dictionary<Product, int> TopTen { get; set; }

        public List<ReservedProduct> AllReservedProducts { get; set; }


    }
}
