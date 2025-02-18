using Dapper;
using Microsoft.VisualBasic;
using MyNightDapper.Context;
using MyNightDapper.Dtos;

namespace MyNightDapper.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly DapperContext _context;

        public SalesRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<(List<dynamic> Data, int TotalCount)> GetAllSalesAsync(int page, int pageSize, string searchTerm)
        {
            using var connection = _context.CreateConnection();

            // Filtreleme için Search sorgusu
            string searchCondition = !string.IsNullOrEmpty(searchTerm) ? "WHERE ITEMNAME LIKE @SearchTerm OR CITY LIKE @SearchTerm OR BRAND LIKE @SearchTerm" : "";

            // Toplam veri sayısını almak için COUNT
            string countQuery = $"SELECT COUNT(1) FROM Sales {searchCondition}";
            int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { SearchTerm = $"%{searchTerm}%" });

            // Daha hızlı çalışması için sadece gerekli sütunları seçiyoruz
            string query = $@"
    SELECT ID, ITEMNAME, UNITPRICE, AMOUNT, TOTALPRICE, CITY, BRAND, DATE_
    FROM Sales WITH (NOLOCK) 
    {searchCondition}
    ORDER BY DATE_ DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var result = await connection.QueryAsync<dynamic>(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                SearchTerm = $"%{searchTerm}%"
            });

            return (result.ToList(), totalCount);
        }

        public async Task<object> GetSalesByAgeGroupAsync()
        {
            string query = @"
           SELECT 
    CASE 
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 18 AND 25 THEN '18-25'
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 26 AND 35 THEN '26-35'
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 36 AND 45 THEN '36-45'
        ELSE '46+' 
    END AS AgeGroup,
    SUM(TOTALPRICE) AS TotalRevenue
FROM SALES
GROUP BY 
    CASE 
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 18 AND 25 THEN '18-25'
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 26 AND 35 THEN '26-35'
        WHEN DATEDIFF(YEAR, USERBIRTHDATE, GETDATE()) BETWEEN 36 AND 45 THEN '36-45'
        ELSE '46+' 
    END
ORDER BY TotalRevenue DESC";

            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }

        public async Task<List<dynamic>> GetSalesByBrandAsync()
        {
            string query = @"
        SELECT BRAND, SUM(TOTALPRICE) AS TotalRevenue
        FROM Sales
        GROUP BY BRAND
        ORDER BY TotalRevenue DESC";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }

        public async Task<object> GetSalesByCategoryAsync()
        {
            string query = @"
            SELECT CATEGORY1, SUM(TOTALPRICE) AS TotalRevenue
            FROM SALES
            GROUP BY CATEGORY1
            ORDER BY TotalRevenue DESC";
            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }

        public async Task<object> GetSalesByCityAsync()
        {
            string query = @"
            SELECT CITY, SUM(TOTALPRICE) AS TotalRevenue
            FROM SALES
            GROUP BY CITY
            ORDER BY TotalRevenue DESC";
            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }

        public async Task<object> GetSalesByUnitPriceAsync()
        {
            string query = @"
            SELECT UNITPRICE, SUM(AMOUNT) AS TotalAmount
            FROM SALES
            GROUP BY UNITPRICE
            ORDER BY UNITPRICE";

            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }

        public async Task<object> GetTopSellingProductsAsync()
        {
            string query = @"
    SELECT TOP 10 ITEMNAME, SUM(AMOUNT) AS TotalAmount
    FROM SALES
    GROUP BY ITEMNAME
    ORDER BY TotalAmount DESC";

            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(query);
            return result.ToList();
        }
    }
}
