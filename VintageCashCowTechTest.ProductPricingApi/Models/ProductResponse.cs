namespace VintageCashCowTechTest.ProductPricingApi.Models
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
