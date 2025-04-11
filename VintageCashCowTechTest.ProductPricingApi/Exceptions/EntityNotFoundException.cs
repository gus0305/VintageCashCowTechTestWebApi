namespace VintageCashCowTechTest.ProductPricingApi.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityType, string entityId) : base($"{entityType}: {entityId} not found") { }
    }
}
