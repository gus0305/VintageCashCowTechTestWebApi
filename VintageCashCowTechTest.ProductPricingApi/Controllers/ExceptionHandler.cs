using Microsoft.AspNetCore.Mvc;
using System.Net;
using VintageCashCowTechTest.ProductPricingApi.Exceptions;

namespace VintageCashCowTechTest.ProductPricingApi.Controllers
{
    public class ExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public ActionResult Handle(Exception exception)
        {
            if (exception is EntityNotFoundException entityNotFoundException)
            {
                return new NotFoundObjectResult(entityNotFoundException.Message);
            }
            if (exception is ValidationException validationException)
            {
                return new BadRequestObjectResult(validationException.Message);
            }

            _logger.LogError(exception, exception.Message);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
