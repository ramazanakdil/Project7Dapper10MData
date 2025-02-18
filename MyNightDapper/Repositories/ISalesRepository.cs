using MyNightDapper.Dtos;

namespace MyNightDapper.Repositories
{
    public interface ISalesRepository
    {
        Task<object> GetTopSellingProductsAsync();
        Task<object> GetSalesByCityAsync();
        Task<object> GetSalesByAgeGroupAsync();
        Task<object> GetSalesByCategoryAsync();
        Task<object> GetSalesByUnitPriceAsync();
        Task<List<dynamic>> GetSalesByBrandAsync();
        Task<(List<dynamic> Data, int TotalCount)> GetAllSalesAsync(int page, int pageSize, string searchTerm);
    }
}
