namespace VintageCashCowTechTest.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();
    }
}
