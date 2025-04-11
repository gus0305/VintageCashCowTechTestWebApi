using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Services
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetProductsAsync();
        Task<ProductPriceHistoryResponse> GetProductPriceHistoryAsync(int id);
        Task<DiscountResponse> ApplyDiscountAsync(int id, DiscountRequest request);
        Task<UpdatePriceResponse> UpdatePriceAsync(int id, UpdatePriceRequest request);
    }
}
