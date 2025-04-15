using VintageCashCowTechTest.Domain.Entities;

namespace VintageCashCowTechTest.Domain.Repositories
{
    public interface IProductRepository
    {
        public List<Product> GetAll();
        public Product? GetById(int id);
        public void Save(Product product);
    }
}
