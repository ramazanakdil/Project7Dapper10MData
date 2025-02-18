using Microsoft.AspNetCore.Mvc;
using MyNightDapper.Models;
using MyNightDapper.Repositories;


namespace MyNightDapper.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISalesRepository _salesRepository;

        public DashboardController(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }


        public async Task<IActionResult> Index()
        {
            // Verileri Repository'den alıyoruz
            var topSellingProducts = await _salesRepository.GetTopSellingProductsAsync();
            var salesByCity = await _salesRepository.GetSalesByCityAsync();
            var salesByAgeGroup = await _salesRepository.GetSalesByAgeGroupAsync();
            var salesByCategory = await _salesRepository.GetSalesByCategoryAsync();
            var salesByUnitPrice = await _salesRepository.GetSalesByUnitPriceAsync();
            var salesByBrand = await _salesRepository.GetSalesByBrandAsync();




            // ViewBag ile verileri view'a gönderiyoruz


            ViewBag.TopSellingProducts = topSellingProducts;
            ViewBag.SalesByCity = salesByCity;
            ViewBag.SalesByAgeGroup = salesByAgeGroup;
            ViewBag.SalesByCategory = salesByCategory;
            ViewBag.SalesByUnitPrice = salesByUnitPrice;
            ViewBag.SalesByBrand = salesByBrand;

            return View();
        }

        public async Task<IActionResult> AllSalesPages(int page = 1, string searchTerm = "")
        {
            int pageSize = 20;
            var (sales, totalCount) = await _salesRepository.GetAllSalesAsync(page, pageSize, searchTerm);

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            return View(sales);
        }
    }
}
