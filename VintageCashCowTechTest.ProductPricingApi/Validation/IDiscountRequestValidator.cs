using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Validation
{
    public interface IDiscountRequestValidator
    {
        void Validate(DiscountRequest discountRequest);
    }
}