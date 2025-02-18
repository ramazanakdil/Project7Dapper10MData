namespace MyNightDapper.Models
{
    public class DashboardViewModel
    {
        public List<dynamic> TopSellingProducts { get; set; }
        public List<dynamic> SalesByCity { get; set; }
        public List<dynamic> SalesByAgeGroup { get; set; }
        public List<dynamic> SalesByCategory { get; set; }
        public List<dynamic> SalesByUnitPrice { get; set; }
    }
}
