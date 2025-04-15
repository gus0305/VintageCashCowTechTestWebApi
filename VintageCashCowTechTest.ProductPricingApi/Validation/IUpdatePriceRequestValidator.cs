using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Validation
{
    public interface IUpdatePriceRequestValidator
    {
        void Validate(UpdatePriceRequest updatePriceRequest);
    }
}