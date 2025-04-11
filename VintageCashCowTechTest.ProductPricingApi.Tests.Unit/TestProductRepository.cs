using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.Domain.Entities;

namespace VintageCashCowTechTest.ProductPricingApi.Tests.Integration
{
    public class TestProductRepository : IProductRepository
    {
        private List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 11,
                Name = "Product A11",
                Price = 100.0m,
                LastUpdated = DateTime.Parse("2024-09-26T12:34:56"),
                PriceHistory = new List<PriceHistory>
                {
                    new PriceHistory { Price = 120.0m, Date = DateTime.Parse("2024-09-01") },
                    new PriceHistory { Price = 110.0m, Date = DateTime.Parse("2024-08-15") },
                    new PriceHistory { Price = 100.0m, Date = DateTime.Parse("2024-08-10") }
                }
            },
            new Product
            {
                Id = 22,
                Name = "Product B22",
                Price = 200.0m,
                LastUpdated = DateTime.Parse("2024-09-25T10:12:34"),
                PriceHistory = new List<PriceHistory>()
            }
        };

        public List<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(x => x.Id == id);
        }
    }
}

