using Microsoft.ApplicationInsights.DataContracts;
using VintageCashCowTechTest.Infrastructure;
using VintageCashCowTechTest.ProductPricingApi.Calculators;
using VintageCashCowTechTest.ProductPricingApi.Controllers;
using VintageCashCowTechTest.ProductPricingApi.Mappers;
using VintageCashCowTechTest.ProductPricingApi.Services;
using VintageCashCowTechTest.ProductPricingApi.Validation;

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

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddInfrastructure();
            builder.Services.AddScoped<ExceptionHandler>();
            builder.Services.AddScoped<IUpdatePriceRequestValidator, UpdatePriceRequestValidator>();
            builder.Services.AddScoped<IDiscountRequestValidator, DiscountRequestValidator>();
            builder.Services.AddScoped<IProductResponseMapper, ProductResponseMapper>();
            builder.Services.AddScoped<IProductPriceHistoryMapper, ProductPriceHistoryMapper>();
            builder.Services.AddScoped<IDiscountedPriceCalculator, DiscountedPriceCalculator>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();
            app.Use(WebUiRequestTelemetryMiddleware());

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

        private static Func<RequestDelegate, RequestDelegate> WebUiRequestTelemetryMiddleware()
        {
            return next => async httpContext =>
            {
                httpContext.Response.OnStarting(() =>
                {
                    var requestId = Guid.NewGuid().ToString();
                    httpContext.Response.Headers.TryAdd("x-vcc-productapi-requestid", requestId);

                    const string vintageCashCowPrefix = "VintageCashCash";
                    var requestTelemetry = httpContext.Features.Get<RequestTelemetry>();
                    requestTelemetry?.Properties.Add($"{vintageCashCowPrefix}.RequestId", requestId);

                    return Task.CompletedTask;
                });
                await next(httpContext);
            };
        }
    }
}
