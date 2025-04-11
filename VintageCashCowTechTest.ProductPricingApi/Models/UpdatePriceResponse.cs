namespace VintageCashCowTechTest.ProductPricingApi.Models
{
    public class UpdatePriceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
