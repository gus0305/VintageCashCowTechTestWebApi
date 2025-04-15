using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Validation
{
    public class UpdatePriceRequestValidator : IUpdatePriceRequestValidator
    {
        private const int minNewPrice = 0;

        public void Validate(UpdatePriceRequest updatePriceRequest)
        {
            if (updatePriceRequest == null)
            {
                throw new ValidationException("Update price request cannot be null.");
            }

            if (updatePriceRequest.NewPrice < minNewPrice)
            {
                throw new ValidationException("New price cannot be negative.");
            }

            // TODO Max Value?

            // TODO Check for number of decimal places?
        }
    }
}
