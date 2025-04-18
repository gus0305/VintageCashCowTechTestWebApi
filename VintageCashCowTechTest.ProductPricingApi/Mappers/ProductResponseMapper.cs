﻿using VintageCashCowTechTest.Domain.Entities;
using VintageCashCowTechTest.ProductPricingApi.Models;

namespace VintageCashCowTechTest.ProductPricingApi.Mappers
{
    public class ProductResponseMapper : IProductResponseMapper
    {
        public List<ProductResponse> Map(List<Product> products)
        {
            var productResponses = new List<ProductResponse>();
            foreach (var product in products)
            {
                productResponses.Add(Map(product));
            }

            return productResponses;
        }

        private ProductResponse Map(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                LastUpdated = product.LastUpdated
            };
        }
    }
}
