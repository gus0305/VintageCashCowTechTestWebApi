using VintageCashCowTechTest.Infrastructure;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Services;

namespace VintageCashCowTechTest.ProductPricingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructure();   
            builder.Services.AddScoped<IProductResponseMapper, ProductResponseMapper>();
            builder.Services.AddScoped<IProductPriceHistoryMapper, ProductPriceHistoryMapper>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
