namespace VintageCashCowTechTest.ProductPricingApi.Calculators
{
    public class DiscountedPriceCalculator : IDiscountedPriceCalculator
    {
        public decimal Calculate(decimal percentage, decimal price)
        {
            decimal discountedPercentage = 1 - (percentage / 100);
            var discountedPrice = price * discountedPercentage;
            discountedPrice = Math.Round(discountedPrice, 2);

            return discountedPrice;
        }
    }
}
