using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VintageCashCowTechTest.ProductPricingApi.Models;
using System.Net;
using System.Net.Http.Json;
using VintageCashCowTechTest.Domain.Repositories;
using Microsoft.AspNetCore.TestHost;
using Azure;

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

        private async Task<string> GetContentString(HttpResponseMessage httpResponseMessage)
        {
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        private void AssertResponseContainsApiRequestId(HttpResponseMessage httpResponseMessage)
        {
            var headerExists = httpResponseMessage.Headers.TryGetValues("x-vcc-productapi-requestid", out IEnumerable<string>? headerValues);
            Assert.IsTrue(headerExists, "responseHeader exists");

            Assert.IsNotNull(headerValues, "headerValues");
            Assert.AreEqual(1, headerValues.Count(), "headerValues.Count");
            Assert.IsTrue(Guid.TryParse(headerValues.ElementAt(0), out Guid result), "RequestId Guid");
        }

        private async Task AssertResponseIsInternalServerError(HttpResponseMessage httpResponseMessage)
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
            var message = await GetContentString(httpResponseMessage);
            Assert.IsTrue(message.Contains("An error occurred while processing your request."));
        }

        private async Task AssertResponseIsBadRequest(HttpResponseMessage httpResponseMessage, string expectedError)
        {
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            var message = await GetContentString(httpResponseMessage);
            Assert.AreEqual(expectedError, message);
        }

        private async Task AssertResponseIsNotFound(HttpResponseMessage httpResponseMessage, string expectedError)
        {
            Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            var message = await GetContentString(httpResponseMessage);
            Assert.AreEqual(expectedError, message);
        }

        [TestMethod]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync(BaseResource);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode");

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
            Assert.AreEqual(DateTime.Parse("2024-09-25T10:12:34"), secondProduct.LastUpdated);

            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenProductExists_ReturnsPriceHistory()
        {
            // Arrange
            const int productId = 11;

            // Act
            var response = await _client.GetAsync($"{BaseResource}/{productId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode");

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

            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const int productId = 999;

            // Act
            var response = await _client.GetAsync($"{BaseResource}/{productId}");

            // Assert
            await AssertResponseIsNotFound(response, "Product: 999 not found");
            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task GetProductPriceHistory_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            const int productId = TestProductRepository.ProductIdToThrowException;

            // Act
            var response = await _client.GetAsync($"{BaseResource}/{productId}");

            // Assert
            await AssertResponseIsInternalServerError(response);
            AssertResponseContainsApiRequestId(response);
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
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode");

            var content = await response.Content.ReadAsStringAsync();
            var discountResponse = JsonConvert.DeserializeObject<DiscountResponse>(content);

            Assert.IsNotNull(discountResponse);
            Assert.AreEqual(11, discountResponse.Id);
            Assert.AreEqual("Product A11", discountResponse.Name);
            Assert.AreEqual(100.0m, discountResponse.OriginalPrice);
            Assert.AreEqual(90.0m, discountResponse.DiscountedPrice);

            AssertResponseContainsApiRequestId(response);
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
            await AssertResponseIsNotFound(response, "Product: 999 not found");
            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenRequestInvalid_ReturnsBadRequest()
        {
            // Arrange
            var productId = 11;
            var discountRequest = new DiscountRequest { DiscountPercentage = -1 };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            await AssertResponseIsBadRequest(response, "Discount percentage must be between 0 and 100.");
            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task ApplyDiscount_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var productId = TestProductRepository.ProductIdToThrowException;
            var discountRequest = new DiscountRequest { DiscountPercentage = 1 };

            // Act
            var response = await _client.PostAsJsonAsync(CreateApplyDiscountResourceUri(productId), discountRequest);

            // Assert
            await AssertResponseIsInternalServerError(response);
            AssertResponseContainsApiRequestId(response);
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
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode");

            var content = await response.Content.ReadAsStringAsync();
            var updatePriceResponse = JsonConvert.DeserializeObject<UpdatePriceResponse>(content);
            Assert.IsNotNull(updatePriceResponse);
            Assert.AreEqual(11, updatePriceResponse.Id);
            Assert.AreEqual("Product A11", updatePriceResponse.Name);
            Assert.AreEqual(100, updatePriceResponse.NewPrice);
            Assert.IsTrue(updatePriceResponse.LastUpdated >= DateTime.UtcNow.AddSeconds(-5), "LastUpdated");
            Assert.IsTrue(updatePriceResponse.LastUpdated <= DateTime.UtcNow, "LastUpdated");

            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task UpdatePrice_WhenRequestInvalid_ReturnsBadRequest()
        {
            // Arrange
            var productId = 11;
            var updatePriceRequest = new UpdatePriceRequest { NewPrice = -0.01m };

            // Act
            var response = await _client.PutAsJsonAsync(CreateUpdatePriceResourceUri(productId), updatePriceRequest);

            // Assert
            await AssertResponseIsBadRequest(response, "New price cannot be negative.");
            AssertResponseContainsApiRequestId(response);
        }

        [TestMethod]
        public async Task UpdatePrice_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var productId = TestProductRepository.ProductIdToThrowException;
            var updatePriceRequest = new UpdatePriceRequest { NewPrice = 100.0m };

            // Act
            var response = await _client.PutAsJsonAsync(CreateUpdatePriceResourceUri(productId), updatePriceRequest);

            // Assert
            await AssertResponseIsInternalServerError(response);
            AssertResponseContainsApiRequestId(response);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}

