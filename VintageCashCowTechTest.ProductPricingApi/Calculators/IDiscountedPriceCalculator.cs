namespace VintageCashCowTechTest.ProductPricingApi.Calculators
{
    public interface IDiscountedPriceCalculator
    {
        decimal Calculate(decimal percentage, decimal price);
    }
}