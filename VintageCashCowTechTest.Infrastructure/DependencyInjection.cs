using Microsoft.Extensions.DependencyInjection;
using VintageCashCowTechTest.Domain.Repositories;
using VintageCashCowTechTest.Infrastructure.Repositories;

namespace VintageCashCowTechTest.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
