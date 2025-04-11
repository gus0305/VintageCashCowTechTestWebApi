using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductResponseMapper _productResponseMapper;
        private readonly IProductPriceHistoryMapper _productPriceHistoryMapper;

        public ProductService(IProductRepository productRepository, IProductResponseMapper productResponseMapper, IProductPriceHistoryMapper productPriceHistoryMapper)
        {                    
            _productRepository = productRepository;
            _productResponseMapper = productResponseMapper;
            _productPriceHistoryMapper = productPriceHistoryMapper;
        }

        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            var products = await Task.FromResult(_productRepository.GetAll());

            return _productResponseMapper.Map(products);
        }

        private EntityNotFoundException CreateProductNotFoundException(int productId)
        {
            return new EntityNotFoundException("Product", productId.ToString());
        }

        private ValidationException CreateValidationException(string message)
        {
            return new ValidationException(message);
        }

        public async Task<ProductPriceHistoryResponse> GetProductPriceHistoryAsync(int productId)
        {
            var product = await Task.FromResult(_productRepository.GetById(productId));
            if (product == null)
            {
                throw CreateProductNotFoundException(productId);
            }

            return _productPriceHistoryMapper.Map(product);
        }

        public async Task<DiscountResponse> ApplyDiscountAsync(int productId, DiscountRequest request)
        {
            var product = await Task.FromResult(_productRepository.GetById(productId));
            if (product == null)
            {
                throw CreateProductNotFoundException(productId);
            }

            const int minDiscountPercentage = 0;
            const int maxDiscountPercentage = 100;
            if (request.DiscountPercentage < minDiscountPercentage || request.DiscountPercentage > maxDiscountPercentage)
            {
                throw CreateValidationException($"Discount percentage must be between {minDiscountPercentage} and {maxDiscountPercentage}.");
            }

            var originalPrice = product.Price;
            var discountedPrice = originalPrice * (1 - (request.DiscountPercentage / 100));

            // Update product
            product.Price = discountedPrice;
            product.LastUpdated = DateTime.UtcNow;
            product.PriceHistory.Add(new Domain.Entities.PriceHistory { Price = discountedPrice, Date = DateTime.UtcNow });

            return new DiscountResponse
            {
                Id = product.Id,
                Name = product.Name,
                OriginalPrice = originalPrice,
                DiscountedPrice = discountedPrice
            };
        }

        public async Task<UpdatePriceResponse> UpdatePriceAsync(int productId, UpdatePriceRequest request)
        {
            var product = await Task.FromResult(_productRepository.GetById(productId));
            if (product == null)
            {
                throw CreateProductNotFoundException(productId);
            }

            if (request.NewPrice < 0)
            {
                throw CreateValidationException("New price cannot be negative.");
            }
            // TODO No Max Value??

            // TODO Is the price update always performed even if the current price is the same as the new price?

            product.Price = request.NewPrice;
            product.LastUpdated = DateTime.UtcNow;
            product.PriceHistory.Add(new Domain.Entities.PriceHistory { Price = request.NewPrice, Date = DateTime.UtcNow });

            return new UpdatePriceResponse
            {
                Id = product.Id,
                Name = product.Name,
                NewPrice = request.NewPrice,
                LastUpdated = product.LastUpdated
            };
        }
    }
}
