using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Validation
{
    public class DiscountRequestValidator : IDiscountRequestValidator
    {
        private const int minDiscountPercentage = 0;
        private const int maxDiscountPercentage = 100;

        public void Validate(DiscountRequest discountRequest)
        {
            if (discountRequest == null)
            {
                throw new ValidationException("Discount request cannot be null.");
            }

            if (discountRequest.DiscountPercentage < minDiscountPercentage || discountRequest.DiscountPercentage > maxDiscountPercentage)
            {
                throw new ValidationException($"Discount percentage must be between {minDiscountPercentage} and {maxDiscountPercentage}.");
            }
        }
    }
}
