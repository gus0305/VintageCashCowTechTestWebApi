using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Mappers
{
    public class ProductPriceHistoryMapper : IProductPriceHistoryMapper
    {
        public ProductPriceHistoryResponse Map(Product product)
        {
            return new ProductPriceHistoryResponse
            {
                Id = product.Id,
                Name = product.Name,
                PriceHistory = Map(product.PriceHistory)
            };
        }

        private List<Models.PriceHistory> Map(List<Domain.Entities.PriceHistory> priceHistory)
        {
            var modelPriceHistory = new List<Models.PriceHistory>();

            foreach (var priceHistoryItem in priceHistory)
            {
                if (priceHistoryItem != null)
                {
                    modelPriceHistory.Add(new Models.PriceHistory { Price = priceHistoryItem.Price, Date = priceHistoryItem.Date });
                }
            }

            return modelPriceHistory;
        }
    }
}
