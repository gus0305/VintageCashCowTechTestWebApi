using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Mappers
{
    public interface IProductPriceHistoryMapper
    {
        ProductPriceHistoryResponse Map(Product product);
    }
}