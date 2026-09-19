namespace ET3DeliveryPlannerApi.Errors
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public ErrorResponse(
            int statusCode,
            string? message = null)
        {
            StatusCode = statusCode;

            Message = message
                ?? GetErrorMessageForResponseCode(statusCode);
        }

        private static string GetErrorMessageForResponseCode(
            int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You are not authorized",
                404 => "Resource Not Found",
                409 => "Conflict",
                500 => "Internal Server Error",
                _ => "An error occurred"
            };
        }
    }


}

