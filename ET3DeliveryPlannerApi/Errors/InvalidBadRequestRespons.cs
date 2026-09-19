namespace ET3DeliveryPlannerApi.Errors
{
    public class InvalidBadRequestResponse : ErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public InvalidBadRequestResponse(IEnumerable<string> errors) : base(400)
        {
            Errors = errors;
        }
    }
}
