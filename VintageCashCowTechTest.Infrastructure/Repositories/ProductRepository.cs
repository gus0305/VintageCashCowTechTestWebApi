using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.Domain.Repositories;

namespace VintageCashCowTechTest.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Product A",
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
                Id = 2,
                Name = "Product B",
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
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }
}
