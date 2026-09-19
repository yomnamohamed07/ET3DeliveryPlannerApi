using ET3DeliveryPlanner.Data.MappingProfiles;
using ET3DeliveryPlanner.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace ET3DeliveryPlannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripPlannerService _tripPlannerService;

        public TripsController(
            ITripPlannerService tripPlannerService)
        {
            _tripPlannerService = tripPlannerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Trip>>> GetTrips()
        {
            var trips =
                await _tripPlannerService.CreateTripsAsync();

            return Ok(trips);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<TripSummary>> GetSummary()
        {
            var summary =
                await _tripPlannerService.GetSummaryAsync();

            return Ok(summary);
        }
    }
}