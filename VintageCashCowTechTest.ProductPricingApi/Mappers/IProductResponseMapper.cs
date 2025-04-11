using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Mappers
{
    public interface IProductResponseMapper
    {
        List<ProductResponse> Map(List<Product> products);
    }
}