using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VintageCashCowTechTest.ProductPricingApi.Models;
using System.Net;
using System.Net.Http.Json;
using VintageCashCowTechTest.Domain.Repositories;
using Microsoft.AspNetCore.TestHost;

namespace VintageCashCowTechTest.ProductPricingApi.Tests.Integration
{
    [TestClass]
    public class ProductsControllerTests
    {
        private const string BaseResource = "/api/products";
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ProductsControllerTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddScoped<IProductRepository, TestProductRepository>();
                    });
                });
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync(BaseResource);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductResponse>>(content);

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());
            Assert.IsTrue(products.Count == 2);

            var firstProduct = products[0];
            Assert.AreEqual(11, firstProduct.Id);
            Assert.AreEqual("Product A11", firstProduct.Name);
            Assert.AreEqual(100.0m, firstProduct.Price);
            Assert.AreEqual(DateTime.Parse("2024-09-26T12:34:56"), firstProduct.LastUpdated);

            var secondProduct = products[1];
            Assert.AreEqual(22, secondProduct.Id);
            Assert.AreEqual("Product B22", secondProduct.Name);
            Assert.AreEqual(200.00m, secondProduct.Price);
            Assert.AreEqual(DateTime.Parse("2024-09-25T10:12:34"), firstProduct.LastUpdated);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenProductExists_ReturnsPriceHistory()
        {
            // Arrange
            const int productId = 11;

            // Act
            var response = await _client.GetAsync($"{BaseResource}/{productId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var priceHistoryResponse = JsonConvert.DeserializeObject<ProductPriceHistoryResponse>(content);

            Assert.IsNotNull(priceHistoryResponse);
            Assert.AreEqual(11, priceHistoryResponse.Id);
            Assert.AreEqual("Product A11", priceHistoryResponse.Name, "");

            Assert.IsNotNull(priceHistoryResponse.PriceHistory, "PriceHistory");
            Assert.IsTrue(priceHistoryResponse.PriceHistory.Count == 3, "PriceHistory.Count");
            Assert.IsTrue(priceHistoryResponse.PriceHistory.All(x => x != null), "PriceHistory.All");

            var lastPriceHistory = priceHistoryResponse.PriceHistory[2];
            Assert.AreEqual(lastPriceHistory.Price, 100.0m);
            Assert.AreEqual(lastPriceHistory.Date, DateTime.Parse("2024-08-10"));
        }

        private async Task<string> GetContentString(HttpResponseMessage httpResponseMessage)
        {
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const int productId = 999;

            // Act
            var response = await _client.GetAsync($"{BaseResource}/{productId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var message = await GetContentString(response);
            Assert.AreEqual("Product: 999 not found", message);
        }

        private string CreateApplyDiscountResourceUri(int productId)
        {
            return $"{BaseResource}/{productId}/apply-discount";
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenValidRequest_ReturnsDiscountedPrice()
        {
            // Arrange
            var productId = 11;
            var discountRequest = new DiscountRequest { DiscountPercentage = 10 };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var discountResponse = JsonConvert.DeserializeObject<DiscountResponse>(content);

            Assert.IsNotNull(discountResponse);
            Assert.AreEqual(11, discountResponse.Id);
            Assert.AreEqual("Product A11", discountResponse.Name);
            Assert.AreEqual(100.0m, discountResponse.OriginalPrice);
            Assert.AreEqual(90.0m, discountResponse.DiscountedPrice);
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;
            var discountRequest = new DiscountRequest { DiscountPercentage = 1 };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var message = await GetContentString(response);
            Assert.AreEqual("Product: 999 not found", message);
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenDiscountLessThanMinimum_ReturnsBadRequest()
        {
            // Arrange
            var productId = 11;
            var discountRequest = new DiscountRequest { DiscountPercentage = -0.01m };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var message = await GetContentString(response);
            Assert.AreEqual("Discount percentage must be between 0 and 100.", message);
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenDiscountGreaterThanMaximum_ReturnsBadRequest()
        {
            // Arrange
            var productId = 11;
            var discountRequest = new DiscountRequest { DiscountPercentage = 100.01m };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var message = await GetContentString(response);
            Assert.AreEqual("Discount percentage must be between 0 and 100.", message);
        }

        private string CreateUpdatePriceResourceUri(int productId)
        {
            return $"{BaseResource}/{productId}/update-price";
        }

        [TestMethod]
        public async Task UpdatePrice_WhenValidRequest_ReturnsUpdatedPrice()
        {
            // Arrange
            var productId = 11;
            var updatePriceRequest = new UpdatePriceRequest { NewPrice = 100.0m };

            // Act
            var response = await _client.PutAsJsonAsync(CreateUpdatePriceResourceUri(productId), updatePriceRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var updatePriceResponse = JsonConvert.DeserializeObject<UpdatePriceResponse>(content);

            Assert.IsNotNull(updatePriceResponse);
            Assert.AreEqual(11, updatePriceResponse.Id);
            Assert.AreEqual("Product A11", updatePriceResponse.Name);
            Assert.AreEqual(100, updatePriceResponse.NewPrice);
            Assert.IsTrue(updatePriceResponse.LastUpdated >= DateTime.UtcNow.AddSeconds(-5), "LastUpdated");
            Assert.IsTrue(updatePriceResponse.LastUpdated <= DateTime.UtcNow, "LastUpdated");
        }

        [TestMethod]
        public async Task UpdatePrice_WhenValidRequestAndMinimumPrice_ReturnsUpdatedPrice()
        {
            // Arrange
            var productId = 11;
            var updatePriceRequest = new UpdatePriceRequest { NewPrice = 0 };

            // Act
            var response = await _client.PutAsJsonAsync(CreateUpdatePriceResourceUri(productId), updatePriceRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var updatePriceResponse = JsonConvert.DeserializeObject<UpdatePriceResponse>(content);

            Assert.AreEqual(0, updatePriceResponse.NewPrice);
        }

        [TestMethod]
        public async Task UpdatePrice_WhenNegativePrice_ReturnsBadRequest()
        {
            // Arrange
            var productId = 11;
            var updatePriceRequest = new UpdatePriceRequest { NewPrice = -0.01m };

            // Act
            var response = await _client.PutAsJsonAsync(CreateUpdatePriceResourceUri(productId), updatePriceRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var message = await GetContentString(response);
            Assert.AreEqual("New price cannot be negative.", message);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}

