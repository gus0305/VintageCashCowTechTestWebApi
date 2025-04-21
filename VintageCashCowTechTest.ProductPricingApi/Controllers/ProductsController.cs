using Microsoft.AspNetCore.Mvc;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Services;

namespace VintageCashCowTechTest.ProductPricingApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ExceptionHandler _exceptionHandler;

        public ProductsController(IProductService productService, ExceptionHandler exceptionHandler)
        {
            _productService = productService;
            _exceptionHandler = exceptionHandler;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductResponse>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductPriceHistoryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductPriceHistoryResponse>> GetProduct(int id)
        {
            try
            {
                var priceHistory = await _productService.GetProductPriceHistoryAsync(id);
                return Ok(priceHistory);
            }
            catch (Exception ex)
            {
                return _exceptionHandler.Handle(ex);
            }
        }

        [HttpPost("{id}/apply-discount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DiscountResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscountResponse>> ApplyDiscount(int id, [FromBody] DiscountRequest request)
        {
            try
            {
                var response = await _productService.ApplyDiscountAsync(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return _exceptionHandler.Handle(ex);
            }
        }

        [HttpPut("{id}/update-price")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePriceResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdatePriceResponse>> UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
        {
            try
            {
                var response = await _productService.UpdatePriceAsync(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return _exceptionHandler.Handle(ex);
            }
        }
    }
}
