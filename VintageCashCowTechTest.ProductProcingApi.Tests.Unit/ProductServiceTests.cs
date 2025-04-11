using Moq;
using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Services;

namespace VintageCashCowTechTest.ProductProcingApi.Tests.Unit
{
    [TestClass]
    public sealed class ProductServiceTests
    {
        private ProductService _productService;

        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IProductResponseMapper> _productResponseMapperMock;
        private Mock<IProductPriceHistoryMapper> _productPriceHistoryMapperMock;


        [TestInitialize]
        public void TestInitialize()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productResponseMapperMock = new Mock<IProductResponseMapper>();
            _productPriceHistoryMapperMock = new Mock<IProductPriceHistoryMapper>();

            _productService = new ProductService(_productRepositoryMock.Object, _productResponseMapperMock.Object, _productPriceHistoryMapperMock.Object);
        }

        private Product CreateProduct()
        {
            return new Product
            {
                Price = 20,
                LastUpdated = DateTime.UtcNow.AddDays(-10),
                PriceHistory = new List<Domain.Entities.PriceHistory> { new Domain.Entities.PriceHistory () }
            };
        }

        [TestMethod]
        public async Task ApplyDiscountAsync_WhenValidRequest_SetsProductProperties()
        {
            // Arrange
            const int productId = 123;
            var request = new DiscountRequest
            {
                DiscountPercentage = 25
            };

            var product = CreateProduct();
            product.Price = 20;
            _productRepositoryMock.Setup(x => x.GetById(productId)).Returns(product);


            // Act
            await _productService.ApplyDiscountAsync(productId, request);


            // Assert
            Assert.AreEqual(15, product.Price, "product.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(product.LastUpdated), "product.LastUpdated");

            Assert.IsNotNull(product.PriceHistory, "product.PriceHistory");
            Assert.AreEqual(2, product.PriceHistory.Count, "product.PriceHistory.Count");
            var productPriceHistory = product.PriceHistory[1];
            Assert.AreEqual(15, productPriceHistory.Price, "productPriceHistory.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(productPriceHistory.Date), "productPriceHistory.Date");
        }

        private bool DateIsApproxEqualToUtcNow(DateTime value)
        {
            return value >= DateTime.UtcNow.AddSeconds(-5) && value <= DateTime.UtcNow;
        }

        [TestMethod]
        public async Task UpdatePriceAsync_WhenValidRequest_SetsProductProperties()
        {
            // Arrange
            const int productId = 123;
            var request = new UpdatePriceRequest
            {
                NewPrice = 25
            };

            var product = CreateProduct();
            _productRepositoryMock.Setup(x => x.GetById(productId)).Returns(product);

            // Act
            await _productService.UpdatePriceAsync(productId, request);

            // Assert
            Assert.AreEqual(25, product.Price);
            Assert.IsTrue(DateIsApproxEqualToUtcNow(product.LastUpdated), "LastUpdated");

            Assert.IsNotNull(product.PriceHistory, "product.PriceHistory");
            Assert.AreEqual(2, product.PriceHistory.Count, "product.PriceHistory.Count");
            var productPriceHistory = product.PriceHistory[1];
            Assert.AreEqual(25, productPriceHistory.Price, "productPriceHistory.Price");
            Assert.IsTrue(DateIsApproxEqualToUtcNow(productPriceHistory.Date), "productPriceHistory.Date");
        }
    }
}
