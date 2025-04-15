using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.ProductPricingApi.Calculators;
using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Validation;

namespace VintageCashCowTechTest.ProductPricingApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductResponseMapper _productResponseMapper;
        private readonly IProductPriceHistoryMapper _productPriceHistoryMapper;
        private readonly IUpdatePriceRequestValidator _updatePriceRequestValidator;
        private readonly IDiscountRequestValidator _discountRequestValidator;
        private readonly IDiscountedPriceCalculator _discountedPriceCalculator;

        public ProductService(
            IProductRepository productRepository, 
            IProductResponseMapper productResponseMapper, 
            IProductPriceHistoryMapper productPriceHistoryMapper,
            IUpdatePriceRequestValidator updatePriceRequestValidator,
            IDiscountRequestValidator discountRequestValidator,
            IDiscountedPriceCalculator discountedPriceCalculator)
        {                    
            _productRepository = productRepository;
            _productResponseMapper = productResponseMapper;
            _productPriceHistoryMapper = productPriceHistoryMapper;
            _updatePriceRequestValidator = updatePriceRequestValidator;
            _discountRequestValidator = discountRequestValidator;
            _discountedPriceCalculator = discountedPriceCalculator;
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

            _discountRequestValidator.Validate(request);

            var originalPrice = product.Price;
            var discountedPrice = _discountedPriceCalculator.Calculate(request.DiscountPercentage, originalPrice);

            // Update product
            product.Price = discountedPrice;
            product.LastUpdated = DateTime.UtcNow;
            product.PriceHistory.Add(new Domain.Entities.PriceHistory { Price = discountedPrice, Date = DateTime.UtcNow });

            // Persist product
            _productRepository.Save(product);   

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

            _updatePriceRequestValidator.Validate(request);

            // TODO Is the price update always performed even if the current price is the same as the new price?
            product.Price = request.NewPrice;
            product.LastUpdated = DateTime.UtcNow;
            product.PriceHistory.Add(new Domain.Entities.PriceHistory { Price = request.NewPrice, Date = DateTime.UtcNow });

            // Persist product
            _productRepository.Save(product);

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
