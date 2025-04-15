using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Validation;

namespace VintageCashCowTechTest.ProductProcingApi.Tests.Unit.Validation
{
    [TestClass]
    public sealed class DiscountRequestValidatorTests
    {
        private DiscountRequestValidator _discountRequestValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            _discountRequestValidator = new DiscountRequestValidator();
        }

        private DiscountRequest CreateDiscountRequest()
        {
            return new DiscountRequest
            {
                DiscountPercentage = 50
            };
        }

        [TestMethod]
        public void Validate_WhenValidRequest_DoesNotThrowValidationException()
        {
            // Arrange            
            var request = CreateDiscountRequest();

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void Validate_WhenDiscountPercentageEqualToZero_DoesNotThrowValidationException()
        {
            // Arrange            
            var request = CreateDiscountRequest();
            request.DiscountPercentage = 0;

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNull(exception);
        }


        [TestMethod]
        public void Validate_WhenDiscountPercentageEqualToOneHundred_DoesNotThrowValidationException()
        {
            // Arrange            
            var request = CreateDiscountRequest();
            request.DiscountPercentage = 100;

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
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
            var request = CreateDiscountRequest();
            request = null;

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("Discount request cannot be null.", exception.Message);
        }

        [TestMethod]
        public void Validate_WhenDiscountPercentageLessThanZero_ThrowsValidationException()
        {
            // Arrange            
            var request = CreateDiscountRequest();
            request.DiscountPercentage = -1;

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("Discount percentage must be between 0 and 100.", exception.Message);
        }

        [TestMethod]
        public void Validate_WhenDiscountPercentageGreaterThanOneHundred_ThrowsValidationException()
        {
            // Arrange            
            var request = CreateDiscountRequest();
            request.DiscountPercentage = 101;

            // Act
            Exception? exception = null;
            try
            {
                _discountRequestValidator.Validate(request);
            }
            catch (ValidationException ve)
            {
                exception = ve;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("Discount percentage must be between 0 and 100.", exception.Message);
        }
    }
}
