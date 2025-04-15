using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Validation;

namespace VintageCashCowTechTest.ProductProcingApi.Tests.Unit.Validation
{
    [TestClass]
    public sealed class UpdatePriceRequestValidatorTests
    {
        private UpdatePriceRequestValidator _updatePriceRequestValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            _updatePriceRequestValidator = new UpdatePriceRequestValidator();
        }

        private UpdatePriceRequest CreateUpdatePriceRequest()
        {
            return new UpdatePriceRequest
            {
                NewPrice = 100m
            };
        }

        [TestMethod]
        public void Validate_WhenValidRequest_DoesNotThrowValidationException()
        {
            // Arrange            
            var request = CreateUpdatePriceRequest();

            // Act
            Exception? exception = null;
            try
            {
                _updatePriceRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void Validate_WhenRequestNull_ThrowsValidationException()
        {
            // Arrange            
            var request = CreateUpdatePriceRequest();
            request = null;

            // Act
            Exception? exception = null;
            try
            {
                _updatePriceRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("Update price request cannot be null.", exception.Message);
        }


        [TestMethod]
        public void Validate_WhenNewPriceLessThanZero_ThrowsValidationException()
        {
            // Arrange            
            var request = CreateUpdatePriceRequest();
            request.NewPrice = -0.01m;

            // Act
            Exception? exception = null;
            try
            {
                _updatePriceRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("New price cannot be negative.", exception.Message);
        }
    }
}
