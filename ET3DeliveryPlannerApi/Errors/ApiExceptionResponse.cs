namespace ET3DeliveryPlannerApi.Errors
{
    namespace ET3DeliveryPlannerApi.Errors
    {
        public class ApiExceptionResponse : ErrorResponse
        {
            public string? Details { get; set; }

            public ApiExceptionResponse(
                int statusCode,
                string? message = null,
                string? details = null)
                : base(statusCode, message)
            {
                Details = details;
            }
        }
    }
}
