using VintageCashCowTechTest.ProductPricingApi.Calculators;

namespace VintageCashCowTechTest.ProductPricingApi.Tests.Unit.Calculators
{
    [TestClass]
    public sealed class DiscountedPriceCalculatorTests
    {
        private DiscountedPriceCalculator _discountedPriceCalculator;

        [TestInitialize]
        public void TestInitialize()
        {
            _discountedPriceCalculator = new DiscountedPriceCalculator();
        }

        [TestMethod]
        public void Calculate_WhenPriceIsWholeNumber_ReturnsResult()
        {
            // Arrange            
            var percentage = 60;
            var price = 100;

            // Act
            var result = _discountedPriceCalculator.Calculate(percentage, price);

            // Assert
            Assert.AreEqual(40.0m, result);
        }

        [TestMethod]
        public void Calculate_WhenPriceIsNotWholeNumberAndRequiresRoundingDown_ReturnsResultRoundedDown()
        {
            // Arrange            
            var percentage = 33;
            var price = 99.99m;

            // Act
            var result = _discountedPriceCalculator.Calculate(percentage, price);

            // Assert
            // 66.9933
            Assert.AreEqual(66.99m, result);
        }

        [TestMethod]
        public void Calculate_WhenPriceIsNotWholeNumberAndRequiresRoundingUp_ReturnsResultRoundedUp()
        {
            // Arrange            
            var percentage = 33;
            var price = 99.56m;

            // Act
            var result = _discountedPriceCalculator.Calculate(percentage, price);

            // Assert
            // 66.7052
            Assert.AreEqual(66.71m, result);
        }
    }
}
