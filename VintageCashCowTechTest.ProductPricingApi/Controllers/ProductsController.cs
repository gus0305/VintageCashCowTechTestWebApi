﻿using Microsoft.AspNetCore.Mvc;
using VintageCashCowTechTest.ProductPricingApi.Exceptions;
using VintageCashCowTechTest.ProductPricingApi.Models;
using VintageCashCowTechTest.ProductPricingApi.Services;

namespace VintageCashCowTechTest.ProductPricingApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPriceHistoryResponse>> GetProduct(int id)
        {
            try
            {
                var priceHistory = await _productService.GetProductPriceHistoryAsync(id);
                return Ok(priceHistory);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/apply-discount")]
        public async Task<ActionResult<DiscountResponse>> ApplyDiscount(int id, [FromBody] DiscountRequest request)
        {
            try
            {
                var response = await _productService.ApplyDiscountAsync(id, request);
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/update-price")]
        public async Task<ActionResult<UpdatePriceResponse>> UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
        {
            try
            {
                var response = await _productService.UpdatePriceAsync(id, request);
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
