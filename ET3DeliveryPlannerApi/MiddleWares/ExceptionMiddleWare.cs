using ET3DeliveryPlanner.Services.Exceptions;
using ET3DeliveryPlannerApi.Errors;
using ET3DeliveryPlannerApi.Errors.ET3DeliveryPlannerApi.Errors;
using System.Net;
using System.Text.Json;

namespace ET3DeliveryPlannerApi.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleWare(
            RequestDelegate next,
            ILogger<ExceptionMiddleWare> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    NotFoundException =>
                        (int)HttpStatusCode.NotFound,

                    BadRequestException =>
                        (int)HttpStatusCode.BadRequest,

                    ConflictException =>
                        (int)HttpStatusCode.Conflict,

                    _ =>
                        (int)HttpStatusCode.InternalServerError
                };

                var response = _env.IsDevelopment()
                    ? new ApiExceptionResponse(
                        context.Response.StatusCode,
                        ex.Message,
                        ex.StackTrace)

                    : new ApiExceptionResponse(
                        context.Response.StatusCode,
                        context.Response.StatusCode ==
                        (int)HttpStatusCode.InternalServerError
                            ? null
                            : ex.Message);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase
                };

                var jsonResponse =
                    JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}