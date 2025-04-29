using Moq;
using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.ProductPricingApi.Calculators;
using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Services;
using VintageCashCowTechTest.ProductPricingApi.Validation;

namespace VintageCashCowTechTest.ProductPricingApi.Tests.Unit.Services
{
    [TestClass]
    public sealed class ProductServiceTests
    {
        private ProductService _productService;

        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IProductResponseMapper> _productResponseMapperMock;
        private Mock<IProductPriceHistoryMapper> _productPriceHistoryMapperMock;
        private Mock<IUpdatePriceRequestValidator> _updatePriceRequestValidatorMock;
        private Mock<IDiscountRequestValidator> _discountRequestValidatorMock;
        private Mock<IDiscountedPriceCalculator> _discountedPriceCalculatorMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productResponseMapperMock = new Mock<IProductResponseMapper>();
            _productPriceHistoryMapperMock = new Mock<IProductPriceHistoryMapper>();
            _updatePriceRequestValidatorMock = new Mock<IUpdatePriceRequestValidator>();
            _discountRequestValidatorMock = new Mock<IDiscountRequestValidator>();
            _discountedPriceCalculatorMock = new Mock<IDiscountedPriceCalculator>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _productResponseMapperMock.Object,
                _productPriceHistoryMapperMock.Object,
                _updatePriceRequestValidatorMock.Object,
                _discountRequestValidatorMock.Object,
                _discountedPriceCalculatorMock.Object);
        }

        private Product CreateProduct(int id)
        {
            return new Product
            {
                Id = id,
                Price = 20,
                Name = "Name" + id,
                LastUpdated = DateTime.UtcNow.AddDays(-10),
                PriceHistory = new List<Domain.Entities.PriceHistory> { new Domain.Entities.PriceHistory() }
            };
        }

        [TestMethod]
        public async Task GetProducts_WhenValidRequest_ReturnsResponse()
        {
            // Arrange
            var products = new List<Product>();
            _productRepositoryMock.Setup(x => x.GetAll()).Returns(products);

            var mappedProducts = new List<ProductResponse>();
            _productResponseMapperMock.Setup(x => x.Map(products)).Returns(mappedProducts);

            // Act
            var response = await _productService.GetProductsAsync();

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(mappedProducts, response);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenValidRequest_ReturnsResponse()
        {
            // Arrange
            const int productId = 123;

            var product = CreateProduct(productId);
            _productRepositoryMock.Setup(x => x.GetById(productId)).Returns(product);

            var productPriceHistoryResponse = new ProductPriceHistoryResponse();
            _productPriceHistoryMapperMock.Setup(x => x.Map(product)).Returns(productPriceHistoryResponse);

            // Act
            var response = await _productService.GetProductPriceHistoryAsync(productId);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(productPriceHistoryResponse, response);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenProductDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            const int productId = 123;

            // Act
            Exception? exception = null;
            try
            {
                await _productService.GetProductPriceHistoryAsync(productId);
            }
            catch (EntityNotFoundException ex)
            {
                exception = ex;
            }

            // Assert      
            Assert.IsNotNull(exception);
            Assert.AreEqual("Product: 123 not found", exception.Message);
        }

        [TestMethod]
        public async Task ApplyDiscountAsync_WhenValidRequest_ReturnsResponse()
        {
            // Arrange
            const int productId = 123;
            var request = new DiscountRequest
            {
                DiscountPercentage = 25
            };

            const decimal productPrice = 20;
            const string productName = "Name 123";
            var product = CreateProduct(productId);
            product.Price = productPrice;
            product.Name = productName;
            _productRepositoryMock.Setup(x => x.GetById(productId)).Returns(product);

            const decimal discountedPrice = 15;
            _discountedPriceCalculatorMock.Setup(x => x.Calculate(request.DiscountPercentage, productPrice)).Returns(discountedPrice);

            // Act
            var response = await _productService.ApplyDiscountAsync(productId, request);

            // Assert
            _discountRequestValidatorMock.Verify(x => x.Validate(request), Times.Exactly(1));

            Assert.AreEqual(discountedPrice, product.Price, "product.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(product.LastUpdated), "product.LastUpdated");

            Assert.IsNotNull(product.PriceHistory, "product.PriceHistory");
            Assert.AreEqual(2, product.PriceHistory.Count, "product.PriceHistory.Count");
            var productPriceHistory = product.PriceHistory[1];
            Assert.AreEqual(productPrice, productPriceHistory.Price, "productPriceHistory.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(productPriceHistory.Date), "productPriceHistory.Date");

            _productRepositoryMock.Verify(x => x.Save(product), Times.Exactly(1));

            Assert.IsNotNull(response, "Response");
            Assert.AreEqual(productId, response.Id, "Response.Id");
            Assert.AreEqual(productName, response.Name, "Response.Name");
            Assert.AreEqual(productPrice, response.OriginalPrice, "Response.OriginalPrice");
            Assert.AreEqual(discountedPrice, response.DiscountedPrice, "Response.DiscountedPrice");
        }

        [TestMethod]
        public async Task ApplyDiscountAsync_WhenProductDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            const int productId = 123;
            var request = new DiscountRequest();

            // Act
            Exception? exception = null;
            try
            {
                await _productService.ApplyDiscountAsync(productId, request);
            }
            catch (EntityNotFoundException ex)
            {
                exception = ex;
            }

            // Assert      
            Assert.IsNotNull(exception);
            Assert.AreEqual("Product: 123 not found", exception.Message);
        }

        private bool DateIsApproxEqualToUtcNow(DateTime value)
        {
            return value >= DateTime.UtcNow.AddSeconds(-5) && value <= DateTime.UtcNow;
        }

        [TestMethod]
        public async Task UpdatePriceAsync_WhenValidRequest_ReturnsResponse()
        {
            // Arrange
            const int productId = 123;
            const decimal newPrice = 25;
            var request = new UpdatePriceRequest
            {
                NewPrice = newPrice
            };

            const string productName = "Name 123";
            var product = CreateProduct(productId);
            product.Price = 20;
            product.Name = productName;
            _productRepositoryMock.Setup(x => x.GetById(productId)).Returns(product);

            // Act
            var response = await _productService.UpdatePriceAsync(productId, request);

            // Assert
            _updatePriceRequestValidatorMock.Verify(x => x.Validate(request), Times.Exactly(1));

            Assert.AreEqual(newPrice, product.Price);
            Assert.IsTrue(DateIsApproxEqualToUtcNow(product.LastUpdated), "LastUpdated");

            Assert.IsNotNull(product.PriceHistory, "product.PriceHistory");
            Assert.AreEqual(2, product.PriceHistory.Count, "product.PriceHistory.Count");
            var productPriceHistory = product.PriceHistory[1];
            Assert.AreEqual(20, productPriceHistory.Price, "productPriceHistory.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(productPriceHistory.Date), "productPriceHistory.Date");

            _productRepositoryMock.Verify(x => x.Save(product), Times.Exactly(1));

            Assert.IsNotNull(response, "Response");
            Assert.AreEqual(productId, response.Id, "Response.Id");
            Assert.AreEqual(productName, response.Name, "Response.Name");
            Assert.AreEqual(newPrice, response.NewPrice, "Response.NewPrice");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(response.LastUpdated), "Response.LastUpdated");
        }

        [TestMethod]
        public async Task UpdatePriceAsync_WhenProductDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            const int productId = 123;
            var request = new UpdatePriceRequest();

            // Act
            Exception? exception = null;
            try
            {
                await _productService.UpdatePriceAsync(productId, request);
            }
            catch (EntityNotFoundException ex)
            {
                exception = ex;
            }

            // Assert      
            Assert.IsNotNull(exception);
            Assert.AreEqual("Product: 123 not found", exception.Message);
        }
    }
}
